using EasySaveWPF.Model;
using EasySaveWPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveWPF.Commands
{
    public class RunJobCommand : CommandBase
    {
        private readonly IBackupService _backupService;

        public RunJobCommand(IBackupService backupService)
        {
            _backupService = backupService;
        }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter);
        }

        public override async void Execute(object parameter)
        {
            if (parameter is BackupJob job)
            {
                await Task.Run(() => _backupService.ExecuteBackupJob(job));
            }
        }
    }
}
