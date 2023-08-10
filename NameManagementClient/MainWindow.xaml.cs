using NameManagementClient;
using Newtonsoft.Json;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NameManagementServer
{
    public partial class MainWindow : Window
    {
        private SimpleTcpClient tcpClient;
        private List<Record> dataList = new List<Record>();

        private readonly TimeSpan serverTimeout = TimeSpan.FromSeconds(10);
        private readonly System.Timers.Timer ServerRespondTimer = new System.Timers.Timer();

        public MainWindow()
        {
            InitializeComponent();
            tcpClient = new SimpleTcpClient();
            tcpClient.DataReceived += TcpClient_DataReceived;
            ServerRespondTimer.Interval = serverTimeout.TotalMilliseconds;
            ServerRespondTimer.Elapsed += ServerRespondTimer_Elapsed;
        }

        // CONNECTION ///////////////////////////////////////////////////////////////
        // /////////////////////////
        private async void btnConnectToSrv_Click(object sender, RoutedEventArgs e)
        {
            string ipAddress = getIp.Text;
            int portNumber = int.Parse(getPort.Text);

            try
            {
                switch (btnConnectToSrv.Content)
                {
                    case "Connect":
                        lblGetConnectStatus.Content = "Connecting...";
                        await ConnectToServerAsync(ipAddress, portNumber);
                        lblGetConnectStatus.Content = "Connected to the server";
                        btnConnectToSrv.Content = "Disconnect";
                        break;
                    case "Disconnect":
                        lblGetConnectStatus.Content = "Disconnecting...";
                        DisconnectFromServer();
                        lblGetConnectStatus.Content = "Not connected";
                        btnConnectToSrv.Content = "Connect";
                        dataGrid.ItemsSource = null;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server is offline" + Environment.NewLine + ex.Message);
                lblGetConnectStatus.Content = "Not connected";
            }
        }
        private async Task ConnectToServerAsync(string ipAddress, int portNumber)
        {
            await Task.Run(() =>
            {
                try
                {
                    tcpClient.Connect(ipAddress, portNumber);
                    byte[] response = tcpClient.WriteLineAndGetReply("ReadAll", TimeSpan.FromSeconds(3)).Data;
                    dataList = DeserializeRecords(response);
                    Dispatcher.Invoke(() => dataGrid.ItemsSource = dataList);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Server is offline " + Environment.NewLine + ex);
                }
            });

            ServerRespondTimer.Start();
        }
        private void DisconnectFromServer()
        {
            tcpClient?.Disconnect();
            ServerRespondTimer.Stop();
        }
        private void ServerRespondTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                lblGetConnectStatus.Content = "Server has gone offline";
                btnConnectToSrv.Content = "Connect";
            });
            DisconnectFromServer();
            ServerRespondTimer.Stop();
        }
        // /////////////////////////
        // CONNECTION ///////////////////////////////////////////////////////////////


        // COMUNICATION /////////////////////////////////////////////////////////////
        // /////////////////////////
        private void TcpClient_DataReceived(object sender, Message e)
        {
            byte[] serverResponse = e.Data;
            ServerRespondTimer.Stop();
            ServerRespondTimer.Start();

            if (Encoding.UTF8.GetString(serverResponse).ToString() == "Connected")
            {
                return;
            }
            else if (Encoding.UTF8.GetString(serverResponse).ToString() == "Editreceived")
            {
                SendMessageToServer(SerializeRecords(dataList));
            }
            else
            {
                try
                {
                    dataList = DeserializeRecords(serverResponse);
                    Dispatcher.Invoke(() => dataGrid.ItemsSource = dataList);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Deserialization error: " + ex.Message);
                }
            }
        }
        private void SendMessageToServer(byte[] message)
        {
            Task.Run(() =>
            {
                string messageStr = Encoding.UTF8.GetString(message);
                try
                {
                    byte[] response = tcpClient.WriteLineAndGetReply(messageStr, TimeSpan.FromSeconds(5)).Data;
                }
                catch (Exception)
                {
                    return;
                }
            });
        }
        // COMUNICATION /////////////////////////////////////////////////////////////
        // /////////////////////////




        // HANDLE DATA /////////////////////////////////////////////////////////
        // /////////////////////////
        private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            updateRowNumbers();
            SendMessageToServer(Encoding.UTF8.GetBytes("Edit"));
        }
        private void updateRowNumbers()
        {
         // EDIT 10.08. 12:40
         // Ted mi doslo, ze tady se melo spis resit jestli ma uzivatel unikatni cislo 
         // a to cislo mu musi zustat i po odebrani jineho radku.
         // Resil bych to tak, ze bych si vytvoril list<int>unikatnicisla tam by se nacetli ID
         // s kazdym dalsimm updatem by se for loopem list prosel dokud by se nenaselo nasledujici unikatni cislo
            for(int i = 0; i < dataList.Count; i++)
            {
                dataList[i].ID = (i+1).ToString();
            }
        }
        private void dataGrid_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            updateRowNumbers();
        }
        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)//RIGHT CLICK DELETE
        {
            if (dataGrid.SelectedItem is Record selectedRecord)
            {
                dataList.Remove(selectedRecord);
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = dataList;
                updateRowNumbers();
                SendMessageToServer(Encoding.UTF8.GetBytes("Edit"));
            }
        }
        private List<Record> DeserializeRecords(byte[] data)
        {
            string jsonString = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<List<Record>>(jsonString);
        }
        private byte[] SerializeRecords(List<Record> UpdatedRecord)
        {
            string jsonData = JsonConvert.SerializeObject(UpdatedRecord);
            return Encoding.UTF8.GetBytes(jsonData);
        }
        // /////////////////////////
        // HANDLE DATA /////////////////////////////////////////////////////////




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
