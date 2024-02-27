using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using EasySaveWPF.Model;
using EasySaveWPF.Services.Interfaces;
using Newtonsoft.Json;

namespace EasySaveWPF.Services
{
    public class ServerService : IServerService
    {
        private static TcpListener _server;
        private static bool _isRunning;
        private static TcpClient _client;
        private static NetworkStream _stream; // Ajout d'un champ pour le flux réseau

        public ServerService()
        {
        }

        public void Start()
        {
            _server = new TcpListener(System.Net.IPAddress.Any, 8888);
            _isRunning = true;
            _server.Start();
            Thread listeningThread = new Thread(ListenForClients);
            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

        private void ListenForClients()
        {
            while (_isRunning)
            {
                try
                {
                    TcpClient client = _server.AcceptTcpClient();
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                    clientThread.IsBackground = true;
                    clientThread.Start(client);
                }
                catch (SocketException ex)
                {
                    MessageBox.Show($"SocketException: {ex.Message}");
                }
            }
        }

        public void SendDataToClients(List<BackupJob> jobs)
        {
            if (_client != null && _client.Connected)
            {
                NetworkStream network_stream = _client.GetStream();
                string jsonData = JsonConvert.SerializeObject(jobs);
                jsonData = "<BOF>" + jsonData + "<EOF>";
                // Envoi des données JSON au client
                byte[] jsonDataBytes = Encoding.ASCII.GetBytes(jsonData);

                network_stream.Write(jsonDataBytes, 0, jsonDataBytes.Length);

            }
        }

        private void HandleClient(object client)
        {
            _client = (TcpClient)client;
            _stream = _client.GetStream();

            List<BackupJob> jobs = JsonConvert.DeserializeObject<List<BackupJob>>(File.ReadAllText(Properties.Settings.Default.JobsFilepath));

            try
            {
                SendDataToClients(jobs);

                while (_isRunning)
                {

                    if (!_client.Connected)
                    {
                        Console.WriteLine("Client disconnected.");
                    }
                    else
                    {
                        if (_stream.DataAvailable)
                        {
                            byte[] data = new byte[256];
                            int bytes = _stream.Read(data, 0, data.Length);
                            string message = Encoding.UTF8.GetString(data, 0, bytes);
                            Console.WriteLine(message);
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while handling the client: {ex.Message}");
                _client.Close();
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _server.Stop();
        }
    }
}
