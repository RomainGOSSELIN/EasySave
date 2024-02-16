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

namespace EasySaveWPF.Commands
{
    public class RunAllJobCommand : CommandBase
    {
        private readonly IBackupService _backupService;
        private readonly IDailyLogService _dailyLogService;

        private readonly ObservableCollection<BackupJob> _backupJobs;


        public RunAllJobCommand(IBackupService backupService, ObservableCollection<BackupJob> backupJobs, IDailyLogService dailyLogService )
        {
            _backupService = backupService;
            _backupJobs = backupJobs;
            _dailyLogService = dailyLogService;
        }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter);
        }

        public override async void Execute(object parameter)
        {
            foreach (BackupJob job in _backupJobs)
            {
                if (job != null)
                {
                    var stopwatch = new Stopwatch();
                    var FileSize = GetDirectorySize(job.SourceDir);

                    stopwatch.Start();
                    await Task.Run(() => _backupService.ExecuteBackupJob(job));
                    stopwatch.Stop();
                    _dailyLogService.AddDailyLog(job, FileSize, (int)stopwatch.ElapsedMilliseconds);

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
