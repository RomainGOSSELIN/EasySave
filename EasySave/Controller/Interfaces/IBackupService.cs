using EasySave.Model;

namespace EasySave.Controller.Interfaces
{
    public interface IBackupService
    {
        void ExecuteBackupJob(BackupJob job);
    }
}