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

            if (processes.Length == 0)
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

                if (processes.Length == 0)
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
                else
                {
                    string messageBoxText = $"Logiciel Métier en cours d'exécution. Impossible de lancer {job.Name}";
                    string caption = "Erreur";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Warning;

                    MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
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
