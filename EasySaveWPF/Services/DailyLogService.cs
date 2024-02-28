

using EasySaveWPF.Model;
using EasySaveWPF.Model.LogFactory;
using System.Globalization;
using static EasySaveWPF.Model.Enum;

namespace EasySaveWPF.Services.Interfaces
{
    public class DailyLogService : IDailyLogService
    {
        private string _dailyLogPath;
        private LoggerContext _logger;
        private static string _logType;
        public DailyLogService(LoggerContext logger)
        {
            _logger = logger;
            _dailyLogPath = Properties.Settings.Default.LogFilePath;
            _logType = Properties.Settings.Default.LogType;
            
            //_logger = _loggerFactory.CreateLogger((Model.Enum.LogType)System.Enum.Parse(typeof(Model.Enum.LogType), logType, true)) ;
        }

        public void AddDailyLog(BackupJob job, long fileSize, int transferTime , long encryptTime)
        {
            if (_logType == "json")
            {
                _logger.SetStrategy(new JsonService());
            }
            else
            {
                _logger.SetStrategy(new XamlService());
            }

            List<BackupLog> logs = _logger.Get<BackupLog>(_dailyLogPath);

            var newlog = new BackupLog(job.Name, DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), CultureInfo.InvariantCulture), job.SourceDir, job.TargetDir, fileSize, transferTime, encryptTime);

            logs.Add(newlog);

            _logger.Save(logs, _dailyLogPath);

        }





    }
}
