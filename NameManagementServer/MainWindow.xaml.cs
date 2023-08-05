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

namespace NameManagementServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SimpleTcpServer server;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            server = new SimpleTcpServer();
            server.ClientConnected += Server_ClientConnected;
            server.ClientDisconnected += Server_ClientDisconnected;
        }

        private void Server_ClientConnected(object sender, TcpClient client)
        {
            Dispatcher.Invoke(() =>
            {
                panel.Text += GetConnectInfo(client) + " connected" + Environment.NewLine;
            });
        }

        private void Server_ClientDisconnected(object sender, TcpClient client)
        {
            Dispatcher.Invoke(() =>
            {
                panel.Text += GetConnectInfo(client) + " disconnected" + Environment.NewLine;
            });
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
