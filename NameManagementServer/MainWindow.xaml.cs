using Newtonsoft.Json;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NameManagementServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SimpleTcpServer server;

        private readonly List<TcpClient> connectedClients = new List<TcpClient>();
        private readonly TimeSpan liveInterval = TimeSpan.FromSeconds(5);

        // INIT //////////////////////////////////////////////////////////////////////
        // /////////////////////////
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            server = new SimpleTcpServer();
            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.DataReceived += Server_DataReceived;
            Task.Run(ServerIsLiveAsync);
        }
        // /////////////////////////
        // INIT //////////////////////////////////////////////////////////////////////




        // COMUNICATION //////////////////////////////////////////////////////////////
        private void Server_DataReceived(object sender, Message e)
        {
            string clientData = "";
            foreach (var item in Encoding.UTF8.GetString(e.Data))
            {
                clientData += item.ToString();
            }
            clientData = clientData.Substring(0, clientData.Length - 1);

            switch (clientData)
            {
                case "Connected":
                    e.Reply("Connected");
                    break;

                case "Edit":
                    e.Reply(Encoding.UTF8.GetBytes("Editreceived"));
                    break;

                case "ReadAll":
                    List<Record> allRecords = ReadAllRecordsFromCSV();
                    string jsonData = JsonConvert.SerializeObject(allRecords);
                    e.Reply(Encoding.UTF8.GetBytes(jsonData));

                    break;
                default:
                    byte[] data = e.Data;
                    try
                    {
                        List<Record> receivedRecords = DeserializeRecords(data);
                        UpdateCSVWithReceivedRecords(receivedRecords);
                        
                        // PUSH TO ALL CLIENTS
                        byte[] actualData = e.Data;
                        string jsonString = Encoding.UTF8.GetString(actualData);
                        jsonString = jsonString.Substring(0, jsonString.Length - 1);
                        actualData = Encoding.UTF8.GetBytes(jsonString);
                        foreach (var client in connectedClients)
                        {
                           client.GetStream().Write(actualData, 0, actualData.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Deserialization error: " + ex.Message);
                    }
                    break;
            }
        }
        private async Task ServerIsLiveAsync()
        {
            while (true)
            {
                await Task.Delay(liveInterval);

                // Send ping message to all connected clients
                foreach (var client in connectedClients)
                {
                    try
                    {
                        byte[] serverPingingBytes = Encoding.UTF8.GetBytes("ping");
                        client.GetStream().Write(serverPingingBytes, 0, serverPingingBytes.Length);
                    }
                    catch
                    {
                        // Handle the exception (e.g., if the client is disconnected)
                    }
                }
            }
        }
        // COMUNICATION //////////////////////////////////////////////////////////////




        // HANDLE DATA ///////////////////////////////////////////////////////////////
        // /////////////////////////
        private List<Record> DeserializeRecords(byte[] data)
        {
            string jsonString = Encoding.UTF8.GetString(data);
            jsonString = jsonString.Substring(0, jsonString.Length - 1);//handle newline char
            return JsonConvert.DeserializeObject<List<Record>>(jsonString);
        }
        private List<Record> ReadAllRecordsFromCSV()
        {
            string fileName = "names.csv";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            // Check if the CSV file exists
            if (File.Exists(filePath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(filePath);
                    List<Record> records = new List<Record>();

                    foreach (string line in lines)
                    {
                        string[] fields = line.Split(';');
                        if (fields.Length >= 3)
                        {
                            // Create a new Record object and add it to the list
                            Record record = new Record
                            {
                                ID = fields[0],
                                FirstName = fields[1],
                                LastName = fields[2]
                            };

                            records.Add(record);
                        }
                    }

                    return records;
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Error reading data from CSV file: " + ex.Message);
                    return new List<Record>();
                }
            }
            else
            {
                File.Create(filePath);
                return new List<Record>();
            }
        }
        private void UpdateCSVWithReceivedRecords(List<Record> receivedRecords)
        {
            string fileName = "names.csv";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            List<string> lines = new List<string>(); 

            File.WriteAllLines(filePath, new string[0]);//Clear original file

            foreach (var record in receivedRecords)
            {
                string line = $"{record.ID};{record.FirstName};{record.LastName}";
                lines.Add(line);
            }
            File.WriteAllLines(filePath, lines);

            

        }
        // /////////////////////////
        // HANDLE DATA ///////////////////////////////////////////////////////////////




        // CONNECTION ////////////////////////////////////////////////////////////////
        // /////////////////////////
        private void btnSrvStartStop_Click(object sender, RoutedEventArgs e)
        {
            IPAddress ipAddress = IPAddress.Parse(getIp.Text);
            int port = int.Parse(getPort.Text);
            string currentTime = $"{DateTime.Now:HH:mm:ss}";

            switch (btnSrvStartStop.Content)
            {
                case "Start":
                    server.Start(ipAddress, port);
                    btnSrvStartStop.Content = "Stop";
                    break;
                case "Stop":
                    server.Stop();
                    connectedClients.Clear();
                    btnSrvStartStop.Content = "Start";
                    break;
                default:
                    break;
            }

            if (server.IsStarted)
            {
                panel.Text += $"{currentTime} - Server started" + Environment.NewLine;
                lblGetServerStatus.Content = "Server is online";
                lblGetServerStatus.Foreground = Brushes.SpringGreen;
                scrollView.ScrollToBottom();
            }
            else
            {
                panel.Text += $"{currentTime} - Server stopped" + Environment.NewLine;
                lblGetServerStatus.Content = "Server is offline";
                lblGetServerStatus.Foreground = Brushes.Red;
                scrollView.ScrollToBottom();
            }
        }
        private void Server_ClientConnected(object sender, TcpClient client)
        {
            Dispatcher.Invoke(() =>
            {
                panel.Text += GetConnectInfo(client) + " connected" + Environment.NewLine;
            });
            connectedClients.Add(client);
        }
        private void Server_ClientDisconnected(object sender, TcpClient client)
        {
            Dispatcher.Invoke(() =>
            {
                panel.Text += GetConnectInfo(client) + " disconnected" + Environment.NewLine;
            });
            connectedClients.Remove(client);
        }
        private string GetConnectInfo(TcpClient client)
        {
            var clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
            var clientPort = ((IPEndPoint)client.Client.RemoteEndPoint).Port;
            string currentTime = $"{DateTime.Now:HH:mm:ss}";

            return $"{currentTime} - Client {clientIp}:{clientPort}";
        }
        // /////////////////////////
        // CONNECTION ////////////////////////////////////////////////////////////////




        // HANDLE FORM INPUT /////////////////////////////////////////////////////////
        // /////////////////////////
        private void getIp_CheckTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text + e.Text;

            e.Handled = !IsValidIpAddress(newText);
        }
        private bool IsValidIpAddress(string input)
        {
            return Regex.IsMatch(input, @"^(\d{1,3}\.){0,3}\d{0,3}$");
        }
        private void getPort_CheckTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text + e.Text;

            e.Handled = !IsValidPortNumber(newText);
        }
        private bool IsValidPortNumber(string input)
        {
            return Regex.IsMatch(input, @"^\d{0,5}$");
        }
        // /////////////////////////
        // HANDLE FORM INPUT /////////////////////////////////////////////////////////

    }
}
