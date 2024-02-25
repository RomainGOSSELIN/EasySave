//using System;
//using System.IO;
//using System.Net.Sockets;
//using System.Runtime.InteropServices;
//using System.Text.Json;
//using System.Threading.Tasks;

//class Program
//{
//    static async Task Main(string[] args)
//    {
//        [DllImport("kernel32.dll", SetLastError = true)]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        static extern bool AllocConsole();
//        AllocConsole();
//        string serverIp = "127.0.0.1"; // Utilisez l'adresse IP de votre serveur
//        int serverPort = 1234; // Utilisez le port sur lequel votre serveur écoute

//        try
//        {
//            using TcpClient client = new TcpClient();
//            Console.WriteLine($"Tentative de connexion au serveur {serverIp} sur le port {serverPort}...");
//            await client.ConnectAsync(serverIp, serverPort);
//            Console.WriteLine("Connecté au serveur. En attente de données...");

//            using NetworkStream stream = client.GetStream();
//            using MemoryStream memoryStream = new MemoryStream();
//            await stream.CopyToAsync(memoryStream);
//            memoryStream.Position = 0;

//            var jobs = JsonSerializer.Deserialize<dynamic[]>(memoryStream.ToArray());

//            Console.WriteLine("Jobs reçus du serveur:");
//            foreach (var job in jobs)
//            {
//                Console.WriteLine($"ID: {job.Id}, Nom: {job.Name}, Source: {job.SourceDir}, Destination: {job.TargetDir}");
//                // Affichez d'autres propriétés selon le besoin.
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Une erreur est survenue: {ex.Message}");
//        }

//        Console.WriteLine("Appuyez sur une touche pour quitter.");
//        Console.ReadKey();
//    }
//}
