using Newtonsoft.Json;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace NameManagementClient
{
    internal class TcpClientManager
    {
        private SimpleTcpClient tcpClient;
        private readonly TimeSpan serverTimeout = TimeSpan.FromSeconds(10);
        private readonly System.Timers.Timer serverRespondTimer = new System.Timers.Timer();

        public event EventHandler<List<Record>> DataReceived;

        public TcpClientManager()
        {
            serverRespondTimer.Interval = serverTimeout.TotalMilliseconds;
            serverRespondTimer.Elapsed += ServerRespondTimer_Elapsed;
        }

        public async Task ConnectAsync(string ipAddress, int portNumber)
        {
            tcpClient = new SimpleTcpClient();
            tcpClient.DataReceived += Client_DataReceived;

            
            // Send commands to the server and receive replies using Task.Run
            await Task.Run(() =>
            {
                try
                {
                    tcpClient.Connect(ipAddress, portNumber);
                    byte[] response1 = tcpClient.WriteLineAndGetReply("Connected", TimeSpan.FromSeconds(1)).Data;
                    byte[] response2 = tcpClient.WriteLineAndGetReply("ReadAll", TimeSpan.FromSeconds(1)).Data;

                    // Deserialize the responses into List<Record>
                    //List<Record> records1 = DeserializeRecords(response1);
                    List<Record> records2 = DeserializeRecords(response2);

                    // Raise the DataReceived event for each response
                    //DataReceived?.Invoke(this, records1);
                    DataReceived?.Invoke(this, records2);
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Server is offline " + Environment.NewLine + ex);
                    
                }
                
            });

            serverRespondTimer.Start();
        }

        public void Disconnect()
        {
            tcpClient?.Disconnect();
            serverRespondTimer.Stop();
        }

        private void Client_DataReceived(object sender, Message e)
        {
            // The server has responded with data for the "ReadAll" command.
            // Raise the DataReceived event to notify the UI about the received data.
            byte[] serverResponse = e.Data;
            serverRespondTimer.Stop();
            serverRespondTimer.Start();

            if (Encoding.UTF8.GetString(serverResponse).ToString() == "Connected")
            {
                return;
            }

            Debug.WriteLine("Received data: " + Encoding.UTF8.GetString(serverResponse).ToString());
            try
            {
                List<Record> dataList = DeserializeRecords(serverResponse);
                // Raise the DataReceived event with the received records
                DataReceived?.Invoke(this, dataList);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Deserialization error: " + ex.Message);
            }
        }
        private List<Record> DeserializeRecords(byte[] data)
        {
            string jsonString = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<List<Record>>(jsonString);
        }

        private void ServerRespondTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // The server has not sent a heartbeat message within the timeout period.
            // Assume that the server has gone offline.
            Disconnect();
        }

    }
}
