using EasySave.Controller.Interfaces;
using EasySave.Model;
using EasySave.Model.LogFactory;

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasySave.Model.Enum;

namespace EasySave.Controller
{
    public class DailyLogService : IDailyLogService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logService;
        private readonly string _dailyLogPath;
        private readonly LogTypeEnum logType;

        public DailyLogService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dailyLogPath = _configuration["AppConfig:LogFilePath"];
            _logService = LoggerFactory.CreateLogger(logType);
        }

        public void AddDailyLog(BackupJob job, long fileSize, int transferTime)
        {

            List<BackupLog> logs = _logService.GetLog<BackupLog>(_dailyLogPath);

            var newlog = new BackupLog(job.Name, DateTime.Now, job.SourceDir, job.TargetDir, fileSize, transferTime);

            logs.Add(newlog);

            _logService.SaveLog(logs, _dailyLogPath);

        }
    }
}
