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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NameManagementServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SimpleTcpClient tcpClient;
        
        public MainWindow()
        {
            InitializeComponent();
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
                        tcpClient = new SimpleTcpClient();
                        Dispatcher.Invoke(() => lblGetConnectStatus.Content = "Connecting...");
                        tcpClient.Connect(ipAddress, portNumber);
                        await Task.Run(() => tcpClient.WriteLineAndGetReply("Connected", TimeSpan.FromSeconds(1)));
                        lblGetConnectStatus.Content = "Connected to the server";
                        btnConnectToSrv.Content = "Disconnect";
                        break;
                    case "Disconnect":
                        Dispatcher.Invoke(() => lblGetConnectStatus.Content = "Disconnecting...");

                        await Task.Run(() => tcpClient.WriteLineAndGetReply("Connected", TimeSpan.FromSeconds(1)));
                        tcpClient.Disconnect();
                        lblGetConnectStatus.Content = "Not connected";
                        btnConnectToSrv.Content = "Connect";
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server is offline" + Environment.NewLine + ex.Message);
            }
            
            
           
        }
    }
}
