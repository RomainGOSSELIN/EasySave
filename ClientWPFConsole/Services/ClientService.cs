
using ClientWPFConsole.Model;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace ClientWPFConsole.Services
{
    public class ClientService
    {


        private static TcpClient _client ;
        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    StatusChanged?.Invoke(this, value);
                }
            }
        }

        public event EventHandler<List<BackupJob>> DataReceived;
        public event EventHandler<bool> StatusChanged;

        string serverIp = "127.0.0.1";
        int serverPort = 8888;

        public void Connect()
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(serverIp, serverPort);
                IsConnected = true;
                Thread receiveDataThread = new Thread(ReceiveData);
                receiveDataThread.IsBackground = true;
                receiveDataThread.Start();
            }
            catch (Exception ex)
            {
                IsConnected = false;
                Console.WriteLine($"Failed to connect to server: {ex.Message}");
            }
        }

        public void ReceiveData()
        {
            NetworkStream stream = _client.GetStream();
            byte[] bytes = new byte[2048];

            while (_client.Connected)
            {

                StringBuilder data = new StringBuilder();
                int bytesRead;
                try
                {

                    while ((bytesRead = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data.Append(Encoding.ASCII.GetString(bytes, 0, bytesRead));

                        string receivedData = data.ToString();
                        if (receivedData.IndexOf("<EOF>") > -1)
                        {

                            string pattern = @"<BOF>(.*?)<EOF>";
                            Match match = Regex.Match(receivedData, pattern);
                            if (match.Success)
                            {
                                string content = match.Groups[1].Value;
                                var backupJobs = System.Text.Json.JsonSerializer.Deserialize<List<BackupJob>>(content); // Désérialiser JSON
                                DataReceived?.Invoke(this, backupJobs);
                            }
                            data.Clear();


                        }
                    }
                }
                catch (Exception ex)
                {
                    IsConnected = false;
                    Console.WriteLine($"Failed to receive data: {ex.Message}");
                }

            }

            IsConnected = false;
        }



        public void SendDataToServer(string command, BackupJob parameter)
        {
            var job = parameter as BackupJob;

            if (_client != null && _client.Connected)
            {
                NetworkStream network_stream = _client.GetStream();
                var commandWithParams = new { Command = command, Parameter = job.Id };

                var data = JsonSerializer.Serialize(commandWithParams);

                byte[] jsonDataBytes = Encoding.ASCII.GetBytes(data);

                network_stream.Write(jsonDataBytes, 0, jsonDataBytes.Length);

            }
        }

        public void Disconnect()
        {
            if (IsConnected)
            {
                _client.Close();
                IsConnected = false;

            }
            else
            {
                Console.WriteLine("Not connected to server.");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


    }
}
