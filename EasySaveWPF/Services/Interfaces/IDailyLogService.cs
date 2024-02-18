
using EasySaveWPF.Model;

namespace EasySaveWPF.Services.Interfaces
{
    public interface IDailyLogService
    {
        void AddDailyLog(BackupJob job, long fileSize, int transferTime, long encryptTime);
    }
}