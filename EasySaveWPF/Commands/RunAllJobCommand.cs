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
    public class RunAllJobCommand : CommandBase
    {
        private readonly IBackupService _backupService;
        private readonly IDailyLogService _dailyLogService;
        private string _processName;
        private readonly ObservableCollection<BackupJob> _backupJobs;
        string message;
        string caption;
        MessageBoxButton button;
        MessageBoxImage icon;


        public RunAllJobCommand(IBackupService backupService, ObservableCollection<BackupJob> backupJobs, IDailyLogService dailyLogService)
        {
            _backupService = backupService;
            _backupJobs = backupJobs;
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

        public override async void Execute(object parameter)
        {
            foreach (BackupJob job in _backupJobs)
            {
                Process[] processes = Process.GetProcessesByName(_processName);

                if (processes.Length == 0 || _processName == "")
                {

                    if (job != null)
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

                            message = Resources.Translation.backup_success;
                            caption = Resources.Translation.success;
                            button = MessageBoxButton.OK;
                            icon = MessageBoxImage.Information;
                            MessageBox.Show(message, caption, button, icon);

                        }
                        else
                        {
                            message = Resources.Translation.source_directory_doesnt_exist;
                            caption = Resources.Translation.error;
                            button = MessageBoxButton.OK;
                            icon = MessageBoxImage.Warning;
                            MessageBox.Show(message, caption, button, icon, MessageBoxResult.Yes);
                        }
                    }
                }
                else
                {
                    message = Resources.Translation.business_software_running;
                    caption = Resources.Translation.error;
                    button = MessageBoxButton.OK;
                    icon = MessageBoxImage.Warning;
                    MessageBox.Show(message, caption, button, icon, MessageBoxResult.Yes);
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
