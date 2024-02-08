using EasySave.Model;

namespace EasySave.Controller.Interfaces
{
    public interface IDailyLogService
    {
        void AddDailyLog(BackupJob job, long fileSize, int transferTime);
    }
}