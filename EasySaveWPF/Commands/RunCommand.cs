﻿using EasySaveWPF.Model;
using EasySaveWPF.Services;
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
    public class RunCommand : CommandBase
    {
        private readonly IBackupService _backupService;
        private readonly IDailyLogService _dailyLogService;
        private BackupViewModel _backupViewModel;
        private static List<BackupJob> _backupJobs;
        private string _processName;
        Notifications.Notifications notifications = new Notifications.Notifications();
        private readonly object _logLock = new object();
        public RunCommand(IBackupService backupService, IDailyLogService dailyLogService, BackupViewModel vm)
        {
            _backupViewModel = vm;

            _backupJobs = _backupViewModel.BackupJobs;

            _backupService = backupService;

            _dailyLogService = dailyLogService;
            _processName = Path.GetFileNameWithoutExtension(Properties.Settings.Default.BusinessSoftwarePath);
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

                if (job.State.State == Model.Enum.StateEnum.PAUSED)
                {
                    job.State.State = Model.Enum.StateEnum.ACTIVE;
                    _backupService.AddBarrierParticipant();
                    job.ResetEvent.Set();
                }
                else
                {

                    ThreadPool.QueueUserWorkItem(state =>
                    {

                        ExecuteJob(job);
                        lock (executedJobs)
                        {
                            executedJobs.Add(job);
                        }
                        notifications.BackupSuccess(executedJobs);
                    });
                }
            }

            else if (parameter.ToString() == "all")
            {
                var result = notifications.Confirmation();
                if (result == MessageBoxResult.No)
                {
                    return;
                }

                if (_backupViewModel.BackupJobs.Count == 0)
                {
                    notifications.NoJob();
                    return;
                }
                foreach (BackupJob job in _backupViewModel.BackupJobs)
                {

                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        ExecuteJob(job);
                        lock (executedJobs)
                        {
                            executedJobs.Add(job);
                        }
                        notifications.BackupSuccess(executedJobs);

                    });
                }
            }
            else if (parameter.ToString() == "some")
            {
                if (_backupViewModel.FromJob != 0 && _backupViewModel.ToJob != 0)
                {
                    if (_backupViewModel.BackupJobs.Count == 0)
                    {
                        notifications.NoJob();
                        return;
                    }
                    if (_backupViewModel.FromJob > _backupViewModel.ToJob || _backupViewModel.FromJob == _backupViewModel.ToJob || _backupViewModel.FromJob > _backupViewModel.BackupJobs.Count || _backupViewModel.ToJob > _backupViewModel.BackupJobs.Count)
                    {
                        notifications.RangeNotValid();
                        return;
                    }
                    List<BackupJob> selectedJobs = new List<BackupJob>();
                    if (_backupViewModel.RunOperation == "and")
                    {
                        selectedJobs = _backupViewModel.BackupJobs.Where(job => job.Id == _backupViewModel.FromJob || job.Id == _backupViewModel.ToJob).ToList();
                    }
                    else if (_backupViewModel.RunOperation == "to")
                    {
                        selectedJobs = _backupViewModel.BackupJobs.Where(job => job.Id >= _backupViewModel.FromJob && job.Id <= _backupViewModel.ToJob).ToList();
                    }

                    foreach (BackupJob job in selectedJobs)
                    {
                        ThreadPool.QueueUserWorkItem(state =>
                        {
                            ExecuteJob(job);
                            lock (executedJobs)
                            {
                                executedJobs.Add(job);
                            }
                            notifications.BackupSuccess(executedJobs);

                        });
                    }
                    //notifications.BackupSuccess(executedJobs);
                }
                else
                {
                    notifications.RangeNotValid();
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
                    job.ResetEvent = new System.Threading.ManualResetEvent(true);
                    job.CancellationTokenSource = new CancellationTokenSource();


                    stopwatch.Start();
                    _backupService.ExecuteBackupJob(job);
                    stopwatch.Stop();

                    lock (_logLock)
                    {
                        _dailyLogService.AddDailyLog(job, FileSize, (int)stopwatch.ElapsedMilliseconds, job.State.EncryptionTime);
                    }
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