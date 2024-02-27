using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using static EasySaveWPF.Model.Enum;

namespace EasySaveWPF
{
    public partial class MainWindow : Window
    {
        private TcpClient _client;
        private bool _isConnected;

        public MainWindow()
        {
            InitializeComponent();
            string serverIp = "127.0.0.1";
            int serverPort = 8888;
            _client = new TcpClient();
            Connect(serverIp, serverPort);
        }

        public void Connect(string serverIp, int serverPort)
        {
            try
            {
                _client.Connect(serverIp, serverPort);
                _isConnected = true;
                Thread receiveDataThread = new Thread(ReceiveData);
                receiveDataThread.IsBackground = true;
                receiveDataThread.Start();
                serverStateValue.Text = "Connected";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to server: {ex.Message}");
                serverStateValue.Text = "Error";

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
                                var backupJobs = JsonSerializer.Deserialize<List<BackupJob>>(content); // Désérialiser JSON
                                Dispatcher.Invoke(() =>
                                {
                                    DataGridBackupJobs.ItemsSource = backupJobs;
                                });
                            }
                            data.Clear();


                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to receive data: {ex.Message}");
                }

            }

            _isConnected = false;
            Dispatcher.Invoke(() =>
            {
                serverStateValue.Text = "Disconnected";
            });
        }


        public void Disconnect()
        {
            if (_isConnected)
            {
                _client.Close();
                _isConnected = false;

            }
            else
            {
                Console.WriteLine("Not connected to server.");
            }
        }



        //    private async void ConnectToServer()
        //    {


        //        try
        //        {
        //            using var client = new TcpClient();
        //            await client.ConnectAsync(serverIp, serverPort);

        //            using var stream = client.GetStream();
        //            using var memoryStream = new MemoryStream();
        //            await stream.CopyToAsync(memoryStream); // Copie le contenu du flux dans memoryStream.
        //            memoryStream.Position = 0; // Réinitialise la position de memoryStream pour la lecture.


        //            var response = Encoding.UTF8.GetString(memoryStream.ToArray());//Convert Memory
        //            var backupJobs = JsonSerializer.Deserialize<List<BackupJob>>(response);//Désérialisation JSON
        //            Dispatcher.Invoke(() =>
        //            {
        //                DataGridBackupJobs.ItemsSource = backupJobs;
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            Dispatcher.Invoke(() =>
        //            {
        //                DataGridBackupJobs.ItemsSource = $"Une erreur est survenue: {ex.Message}";
        //            });
        //        }
        //    }
        //}
        public class BackupJob
        {
            public int Id { get; set; } = 0;
            public string Name { get; set; } = string.Empty;
            public string SourceDir { get; set; } = string.Empty;
            public string TargetDir { get; set; } = string.Empty;
            public JobTypeEnum Type { get; set; } = JobTypeEnum.differential;
            public BackupState State { get; set; } = new BackupState();



        }

        public class BackupState
        {

            public DateTime Timestamp { get; set; } = DateTime.Now;
            public StateEnum State { get; set; } = StateEnum.END;
            public int TotalFilesToCopy { get; set; } = 0;
            public long TotalFilesSize { get; set; } = 0;
            public int NbFilesLeftToDo { get; set; } = 0;
            public long NbFilesSizeLeftToDo { get; set; } = 0;
            public string SourceFilePath { get; set; } = string.Empty;
            public string TargetFilePath { get; set; } = string.Empty;

            public int FileProgress => TotalFilesToCopy - NbFilesLeftToDo;
            public long FileSizeProgress => TotalFilesSize - NbFilesSizeLeftToDo;

            public double Progress => Math.Round((double)(TotalFilesToCopy - NbFilesLeftToDo) / TotalFilesToCopy * 100) >= 0 ? Math.Round((double)(TotalFilesToCopy - NbFilesLeftToDo) / TotalFilesToCopy * 100) : 0;


        }
    }
}
