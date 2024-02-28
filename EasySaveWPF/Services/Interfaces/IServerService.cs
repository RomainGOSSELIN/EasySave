using EasySaveWPF.Model;
using System.Collections.ObjectModel;
using static EasySaveWPF.Services.ServerService;

namespace EasySaveWPF.Services.Interfaces
{
    public interface IServerService
    {
        void Start();
        void Stop();
         void SendDataToClients(List<BackupJob> jobs);
        event EventHandler<CommandWithParameter> DataReceived;

    }
}