//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EasySave.Controller
//{
//    internal class BackupInfo
//    {
//        public string Name { get; set; }
//        public string FileSource { get; set; }
//        public string FileTarget { get; set; }
//        public long FileSize { get; set; }
//        public double FileTransferTime { get; set; }
//        public DateTime Time { get; set; }

//    }
//    class Logger
//    {
//        private static readonly string logDirectory = "Logs";
//        private static string logFilePath => Path.Combine(logDirectory, $"log_{DateTime.Now:yyyyMMdd}.json");


//        static Logger()
//        {
//            if (!Directory.Exists(logDirectory))
//            {
//                Directory.CreateDirectory(logDirectory);
//            }
//        }

//        public static void Log(BackupInfo backupInfo)
//        {
//            try
//            {
//                // Écriture du message dans le fichier log au format JSON
//                string logJson = JsonConvert.SerializeObject(backupInfo, Formatting.Indented);
//                File.WriteAllText(logFilePath, logJson);

//                Console.WriteLine($"Log file created: {logFilePath}");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Erreur lors de l'écriture dans le fichier log : {ex.Message}");
//            }
//        }
//    }

//    class Program
//    {
//        static void Main()
//        {
//            // Création d'un exemple d'objet BackupInfo
//            BackupInfo backupInfo = new BackupInfo
//            {
//                Name = "Save1",
//                FileSource = @"D:\projet_2\TEST\source1234\fichiertxt_3.txt",
//                FileTarget = @"E:\SAVE\projet_2\TEST\source1234\fichiertxt_3.txt",
//                FileSize = 1045,
//                FileTransferTime = 3.83,
//                Time = DateTime.Now
//            };

//            // Utilisation du logger pour enregistrer les informations dans le journal au format JSON
//            Logger.Log(backupInfo);


//        }
//    }
//}
    