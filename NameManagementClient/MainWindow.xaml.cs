﻿using NameManagementClient;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NameManagementServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TimeSpan serverTimeout = TimeSpan.FromSeconds(10);
        private readonly System.Timers.Timer ServerRespondTimer = new System.Timers.Timer();
        private TcpClientManager tcpClientManager;
        

        public MainWindow()
        {
            InitializeComponent();
            tcpClientManager = new TcpClientManager();
            tcpClientManager.DataReceived += TcpClientManager_DataReceived;
        }
        private void ServerRespondTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // The server has not sent a ping message within the timeout period.
            // Assume that the server has gone offline.
            Dispatcher.Invoke(() => 
            { 
                lblGetConnectStatus.Content = "Server has gone offline";
                btnConnectToSrv.Content = "Connect";
            });
            tcpClientManager.Disconnect();
            ServerRespondTimer.Stop();
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

        private async void btnConnectToSrv_Click(object sender, RoutedEventArgs e)
        {
            string ipAddress = getIp.Text;
            int portNumber = int.Parse(getPort.Text);

            try
            {
                switch (btnConnectToSrv.Content)
                {
                    case "Connect":
                        Dispatcher.Invoke(() => lblGetConnectStatus.Content = "Connecting...");
                        await tcpClientManager.ConnectAsync(ipAddress, portNumber);
                        lblGetConnectStatus.Content = "Connected to the server";
                        btnConnectToSrv.Content = "Disconnect";
                        break;
                    case "Disconnect":
                        Dispatcher.Invoke(() => lblGetConnectStatus.Content = "Disconnecting...");
                        tcpClientManager.Disconnect();
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
        private void TcpClientManager_DataReceived(object sender, List<Record> records)
        {
            // Handle the server response here (e.g., update the UI with the received data)
            ServerRespondTimer.Stop();
            ServerRespondTimer.Start();

            // Use the Dispatcher to update the DataGrid on the UI thread
            Dispatcher.Invoke(() =>
            {
                // Bind the records list to the DataGrid
                dataGrid.ItemsSource = records;
            });
        }



    }
}
