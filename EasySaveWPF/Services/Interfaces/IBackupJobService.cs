using EasySaveWPF.Model;

namespace EasySaveWPF.Services.Interfaces
{
    public interface IBackupJobService
    {
        bool CreateJob(BackupJob backupJob);
        bool DeleteJob(BackupJob backupJob);
        List<BackupJob> GetAllJobs();
        BackupJob? GetJob(int id);
        List<BackupJob> GetJobs(List<int> ids);
        void UpdateJob(BackupJob job);

    }
}