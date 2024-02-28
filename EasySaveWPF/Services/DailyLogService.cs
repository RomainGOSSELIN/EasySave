

using EasySaveWPF.Model;
using EasySaveWPF.Model.LogFactory;
using System.Globalization;
using static EasySaveWPF.Model.Enum;

namespace EasySaveWPF.Services.Interfaces
{
    public class DailyLogService : IDailyLogService
    {
        
           
        private LoggerContext _logger;
        private static string _logType;

        private string _dailyLogPath
        {
            get
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\";
                string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "." + _logType;
                return basePath + fileName;
            }
        }

        public DailyLogService(LoggerContext logger)
        {
            _logger = logger;
            _logType = Properties.Settings.Default.LogType;
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
