using EasySaveWPF.Model;
using EasySaveWPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasySaveWPF.Commands
{
    public class RunJobCommand : CommandBase
    {
        private readonly IBackupService _backupService;
        private readonly IDailyLogService _dailyLogService;
        private string _processName;
        Notifications.Notifications notifications = new Notifications.Notifications();

        public RunJobCommand(IBackupService backupService, IDailyLogService dailyLogService)
        {
            _backupService = backupService;
            _dailyLogService = dailyLogService;
            _processName = Path.GetFileNameWithoutExtension(Properties.Settings.Default.BusinessSoftwarePath);

        }

        public override bool CanExecute(object? parameter)
        {
            Process[] processes = Process.GetProcessesByName(_processName);

            Process test = new Process();
            if (processes.Length == 0 || _processName == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override async void Execute(object parameter)
        {
            List<BackupJob> executedJobs = new List<BackupJob>();
            if (parameter is BackupJob job)
            {
                if (Directory.Exists(job.SourceDir))
                {

                    var stopwatch = new Stopwatch();
                    var FileSize = GetDirectorySize(job.SourceDir);

                    stopwatch.Start();
                    await Task.Run(() => _backupService.ExecuteBackupJob(job));
                    var encryptTime = _backupService.GetEncryptTime();

                    stopwatch.Stop();
                    _dailyLogService.AddDailyLog(job, FileSize, (int)stopwatch.ElapsedMilliseconds, encryptTime);
                    executedJobs.Add(job);
                    notifications.BackupSuccess(executedJobs);

                }
                else
                {
                    Notifications.Notifications notifications = new Notifications.Notifications();
                    notifications.SourceDirNotExist(job.SourceDir);
                }
            }
        }
        public long GetDirectorySize(string path)
        {
            return Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                .Select(file => new FileInfo(file).Length)
                .Sum();
        }
    }
}
