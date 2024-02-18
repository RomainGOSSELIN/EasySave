using EasySaveWPF.Model;

namespace EasySaveWPF.Services.Interfaces
{
    public interface IBackupService
    {
        void ExecuteBackupJob(BackupJob job);
        event EventHandler<BackupState> CurrentBackupStateChanged;
        long GetEncryptTime();

    }
}