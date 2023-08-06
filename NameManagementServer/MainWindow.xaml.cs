using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Net.Http;
using System.IO;
using System.Windows.Media.Media3D;
using Newtonsoft.Json;

namespace NameManagementServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SimpleTcpServer server;
        private readonly NameViewModel nameViewModel = new NameViewModel();

        private readonly List<TcpClient> connectedClients = new List<TcpClient>();
        private readonly TimeSpan liveInterval = TimeSpan.FromSeconds(5);
        public MainWindow()
        {
            InitializeComponent();
            nameViewModel = new NameViewModel();
            DataContext = nameViewModel;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            server = new SimpleTcpServer();
            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;
            server.DataReceived += Server_DataReceived;
            Task.Run(ServerIsLiveAsync);
        }
        private void Server_DataReceived(object sender, Message e)
        {
            // Convert the received data to a string
            string clientData = "";
            foreach (var item in Encoding.UTF8.GetString(e.Data))
            {
                clientData+= item.ToString();
            }
            clientData = clientData.Substring(0, clientData.Length - 1);
            
            // Check the command and call corresponding methods to handle it
            switch (clientData)
            {
                case "Connected":
                    // Acknowledge the "Connected" command by replying with an empty response
                    e.Reply("Connected");
                    break;
                case "ReadAll":
                    // Call a method to read all records from the CSV file
                    // and send the data back to the client
                    List<Record> allRecords = ReadAllRecordsFromCSV();

                    // Serialize the records into bytes and send them back to the client
                    string jsonData = JsonConvert.SerializeObject(allRecords);
                    e.Reply(Encoding.UTF8.GetBytes(jsonData));
                    break;
                default:
                    break;
            }
        }

        private List<Record> ReadAllRecordsFromCSV()
        {
            // Provide the file path to your CSV file
            string filePath = @"C:\Users\beran\Documents\names.csv";

            // Check if the CSV file exists
            if (File.Exists(filePath))
            {
                try
                {
                    // Read all lines from the CSV file
                    string[] lines = File.ReadAllLines(filePath);
                    // Create a list to store the records
                    List<Record> records = new List<Record>();

                    foreach (string line in lines)
                    {
                        // Process the line to extract individual fields (if needed)
                        // For example, split the line based on the CSV delimiter (e.g., semicolon)
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

                    // Return the list of records
                    return records;
                }
                catch (IOException ex)
                {
                    // Handle any file IO errors (e.g., file is in use, not found, etc.)
                    MessageBox.Show("Error reading data from CSV file: " + ex.Message);
                    return new List<Record>();
                }
            }
            else
            {
                MessageBox.Show("CSV file not found");
                return new List<Record>();
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

        private void getIp_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text + e.Text;

            // Allow only digits and dots with the desired format (IP address format)
            e.Handled = !IsValidIpAddress(newText);
        }
        private bool IsValidIpAddress(string input)
        { 
            return Regex.IsMatch(input, @"^(\d{1,3}\.){0,3}\d{0,3}$");
        }

        private void getPort_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text + e.Text;

            // Allow only digits with a maximum length of 5
            e.Handled = !IsValidPortNumber(newText);
        }
        private bool IsValidPortNumber(string input)
        {
            return Regex.IsMatch(input, @"^\d{0,5}$");
        }
    }
}
