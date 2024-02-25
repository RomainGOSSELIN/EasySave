using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EasySaveWPF.Model;
using EasySaveWPF.Services;

namespace EasySaveWPF.Services
{
    public class ServerConsole
    {
        private TcpListener _listener;
        private bool _isRunning;
        private readonly StateLogService _stateLogService;

        public ServerConsole(StateLogService stateLogService)
        {
            _listener = new TcpListener(System.Net.IPAddress.Any, 8888); // IP et port
            _stateLogService = stateLogService;
        }

        public void Start()
        {
            _isRunning = true;
            _listener.Start();
            Task.Run(() => ListenForClients()); // Démarre l'écoute d'une tahce
        }

        private async Task ListenForClients()
        {
            while (_isRunning)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    Task.Run(() => HandleClient(client));
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"SocketException: {ex.Message}");
                }
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            try
            {
                // Lit l'état des sauvegardes depuis le fichier JSON
                string backupStatesJson = File.ReadAllText(_stateLogService.GetStateLogPath());

                byte[] data = Encoding.UTF8.GetBytes(backupStatesJson);

                using var stream = client.GetStream();
                await stream.WriteAsync(data, 0, data.Length);
                //Console.WriteLine("donnés envoyé");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur est survenue lors de la gestion du client : {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();
        }
    }
}
