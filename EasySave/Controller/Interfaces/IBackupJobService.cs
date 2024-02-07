using EasySave.Model;

namespace EasySave.Controller.Interfaces
{
    public interface IBackupJobService
    {
        bool CreateJob(BackupJob backupJob);
        bool DeleteJob(int idToDelete);
        List<BackupJob> GetAllJobs();
        BackupJob? GetJob(int id);
        List<BackupJob> GetJobs(List<int> ids);
    }
}