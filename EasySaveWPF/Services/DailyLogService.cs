

using EasySaveWPF.Model;
using EasySaveWPF.Model.LogFactory;
using System.Globalization;

namespace EasySaveWPF.Services.Interfaces
{
    public class DailyLogService : IDailyLogService
    {
        private string _dailyLogPath;
        private LoggerFactory _loggerFactory = new LoggerFactory();
        private ILogger _logger;
        public DailyLogService()
        {
            _dailyLogPath = Properties.Settings.Default.LogFilePath;
            var logType = Properties.Settings.Default.LogType;
            _logger = _loggerFactory.CreateLogger((Model.Enum.LogType)System.Enum.Parse(typeof(Model.Enum.LogType), logType, true)) ;
        }

        public void AddDailyLog(BackupJob job, long fileSize, int transferTime)
        {

            List<BackupLog> logs = _logger.GetLog<BackupLog>(_dailyLogPath);

            var newlog = new BackupLog(job.Name, DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), CultureInfo.InvariantCulture), job.SourceDir, job.TargetDir, fileSize, transferTime);

            logs.Add(newlog);

            _logger.SaveLog(logs, _dailyLogPath);

        }





    }
}
