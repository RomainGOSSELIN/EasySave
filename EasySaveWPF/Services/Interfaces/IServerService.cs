using EasySaveWPF.Model;

namespace EasySaveWPF.Services.Interfaces
{
    public interface IServerService
    {
        void Start();
        void Stop();
         void SendDataToClients(List<BackupJob> jobs);

    }
}