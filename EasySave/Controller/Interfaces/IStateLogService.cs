using EasySave.Model;

namespace EasySave.Controller.Interfaces
{
    public interface IStateLogService
    {
        void CreateStateLog(BackupJob job);
        void SaveStateLog(string state, string directory);
        void UpdateStateLog(BackupState state);
        void DeleteStateLog(int idToDelete);
    }
}