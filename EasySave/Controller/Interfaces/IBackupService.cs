using EasySave.Model;

namespace EasySave.Controller.Interfaces
{
    internal interface IBackupService
    {
        void ExecuteBackupJob(BackupJob job);
    }
}