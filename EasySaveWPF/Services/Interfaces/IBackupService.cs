using EasySaveWPF.Model;

namespace EasySaveWPF.Services.Interfaces
{
    public interface IBackupService
    {
        void ExecuteBackupJob(BackupJob job);
        event EventHandler<BackupJob> CurrentBackupStateChanged;
        void AddBarrierParticipant();


        void RemoveBarrierParticipant();
    }
}