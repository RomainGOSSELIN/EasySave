using EasySave.Controller.Interfaces;
using EasySave.Model;
using EasySave.Controller;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Controller
{
    public class DailyLogService : IDailyLogService
    {
        private readonly IConfiguration _configuration;
        private readonly JsonService _jsonService = new JsonService();
        private string _dailyLogPath;
        public DailyLogService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dailyLogPath = _configuration["AppConfig:LogFilePath"];
        }

        public void AddDailyLog(BackupJob job, long fileSize, int transferTime)
        {

            List<BackupLog> logs = _jsonService.GetLog<BackupLog>(_dailyLogPath);

            var newlog = new BackupLog(job.Name, DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")), job.SourceDir, job.TargetDir, fileSize, transferTime);

            logs.Add(newlog);

            _jsonService.SaveLog(logs, _dailyLogPath);

        }




    }
}
