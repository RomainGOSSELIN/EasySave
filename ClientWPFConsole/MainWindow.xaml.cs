using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace EasySaveWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private async void ConnectToServer()
        {
            string serverIp = "127.0.0.1";
            int serverPort = 8888; 

            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(serverIp, serverPort);

                using var stream = client.GetStream();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream); // Copie le contenu du flux dans memoryStream.
                memoryStream.Position = 0; // Réinitialise la position de memoryStream pour la lecture.

               
                var response = Encoding.UTF8.GetString(memoryStream.ToArray());//Convert Memory
                var backupJobs = JsonSerializer.Deserialize<List<BackupJob>>(response);//Désérialisation JSON
                Dispatcher.Invoke(() =>
                {
                    DataGridBackupJobs.ItemsSource = backupJobs;
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    DataGridBackupJobs.ItemsSource = $"Une erreur est survenue: {ex.Message}";
                });
            }
        }
    }
    public class BackupJob
    {
        public int Id { get; set; }
        public string BackupName { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public double Progress { get; set; }
    }
    
}
