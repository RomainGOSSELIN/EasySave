
using EasySaveWPF.Model;

namespace EasySaveWPF.Services.Interfaces
{
    public interface IStateLogService
    {
        void CreateStateLog(BackupJob job);
        void SaveStateLog(string state, string directory);
        void UpdateStateLog(BackupState state);
        void DeleteStateLog(BackupJob job);
    }
}