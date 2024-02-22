using EasySaveWPF.Model;
using EasySaveWPF.Services.Interfaces;
using EasySaveWPF.ViewModel;
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
    public class RunFactoCommand : CommandBase
    {
        private readonly IBackupService _backupService;
        private readonly IDailyLogService _dailyLogService;
        private BackupViewModel _backupViewModel;
        private string _processName;
        Notifications.Notifications notifications = new Notifications.Notifications();

        public RunFactoCommand(IBackupService backupService, IDailyLogService dailyLogService, BackupViewModel vm)
        {
            _backupService = backupService;
            _dailyLogService = dailyLogService;
            _processName = Path.GetFileNameWithoutExtension(Properties.Settings.Default.BusinessSoftwarePath);
            _backupViewModel = vm;
        }

        public override bool CanExecute(object? parameter)
        {
            Process[] processes = Process.GetProcessesByName(_processName);

            if (processes.Length == 0 || _processName == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Execute(object parameter)
        {
            List<BackupJob> executedJobs = new List<BackupJob>();

            if (parameter is BackupJob)
            {
                var job = (BackupJob)parameter;
                Thread thread = new Thread(() => ExecuteJob(job)); // Start ExecuteJob in a new thread
                thread.Start();
                executedJobs.Add(job);
                notifications.BackupSuccess(executedJobs);
            }
            else if (parameter.ToString() == "all")
            {
                foreach (BackupJob job in _backupViewModel.BackupJobs)
                {
                    Thread thread = new Thread(() => ExecuteJob(job));
                    thread.Start();
                    executedJobs.Add(job);
                }
                notifications.BackupSuccess(executedJobs);
            }
            else if (parameter.ToString() == "some")
            {
                if (_backupViewModel.FromJob != 0 && _backupViewModel.ToJob != 0)
                {
                    List<BackupJob> selectedJobs = new List<BackupJob>();
                    if (_backupViewModel.RunOperation == "et")
                    {
                        selectedJobs = _backupViewModel.BackupJobs.Where(job => job.Id == _backupViewModel.FromJob || job.Id == _backupViewModel.ToJob).ToList();
                    }
                    else if (_backupViewModel.RunOperation == "à")
                    {
                        selectedJobs = _backupViewModel.BackupJobs.Where(job => job.Id >= _backupViewModel.FromJob && job.Id <= _backupViewModel.ToJob).ToList();
                    }

                    foreach (BackupJob job in selectedJobs)
                    {
                        Thread thread = new Thread(() => ExecuteJob(job));
                        thread.Start();
                        executedJobs.Add(job);
                    }
                    notifications.BackupSuccess(executedJobs);
                }
            }
        }

        public void ExecuteJob(BackupJob job)
        {
            if (job != null)
            {
                if (Directory.Exists(job.SourceDir))
                {
                    var stopwatch = new Stopwatch();
                    var FileSize = GetDirectorySize(job.SourceDir);

                    stopwatch.Start();
                    _backupService.ExecuteBackupJob(job);
                    var encryptTime = _backupService.GetEncryptTime();
                    stopwatch.Stop();
                    _dailyLogService.AddDailyLog(job, FileSize, (int)stopwatch.ElapsedMilliseconds, encryptTime);
                }
                else
                {
                    notifications.SourceDirNotExist(job.SourceDir);
                    return;
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
