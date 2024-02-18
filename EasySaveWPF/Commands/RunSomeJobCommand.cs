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
    public class RunSomeJobCommand : CommandBase
    {
        private readonly IBackupService _backupService;
        private readonly IDailyLogService _dailyLogService;
        private string _processName;
        private readonly ObservableCollection<BackupJob> _backupJobs;
        private BackupViewModel _vm;



        public RunSomeJobCommand(IBackupService backupService, ObservableCollection<BackupJob> backupJobs, IDailyLogService dailyLogService,  BackupViewModel vm)
        {
            _backupService = backupService;
            _backupJobs = backupJobs;
            _dailyLogService = dailyLogService;
            _processName = Path.GetFileNameWithoutExtension(Properties.Settings.Default.BusinessSoftwarePath);
            
            _vm = vm;
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
            if (_vm.FromJob != 0 && _vm.ToJob != 0)
            {
                List<BackupJob> selectedJobs = new List<BackupJob>();
                if (_vm.RunOperation == "et") {
                    selectedJobs = _backupJobs.Where(job => job.Id == _vm.FromJob || job.Id == _vm.ToJob).ToList();
                }
                else if (_vm.RunOperation == "à")
                {
                    selectedJobs = _backupJobs.Where(job => job.Id >= _vm.FromJob && job.Id <= _vm.ToJob).ToList();
                }

                foreach (BackupJob job in selectedJobs)
                {
                    Process[] processes = Process.GetProcessesByName(_processName);

                    if (processes.Length == 0 || _processName == "")
                    {

                        if (job != null)
                        {
                            var stopwatch = new Stopwatch();
                            var FileSize = GetDirectorySize(job.SourceDir);

                            stopwatch.Start();
                            await Task.Run(() => _backupService.ExecuteBackupJob(job));
                            var encryptTime = _backupService.GetEncryptTime();
                            stopwatch.Stop();
                            _dailyLogService.AddDailyLog(job, FileSize, (int)stopwatch.ElapsedMilliseconds, encryptTime);

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
            
        }

        public long GetDirectorySize(string path)
        {
            return Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                .Select(file => new FileInfo(file).Length)
                .Sum();
        }
    }
}
