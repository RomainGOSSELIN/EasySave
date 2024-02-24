using EasySaveWPF.Model;
using EasySaveWPF.Services.Interfaces;
using EasySaveWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveWPF.Commands
{
    public class DeleteJobCommand : CommandBase
    {
        private readonly IBackupJobService _backupJobService;
        private readonly IStateLogService _stateLogService;
        private BackupViewModel _backupViewModel;

        public DeleteJobCommand(IBackupJobService backupJobService, BackupViewModel vm)
        {
            _backupJobService = backupJobService;
            _backupViewModel = vm;
        }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter);
        }

        public override void Execute(object parameter)
        {
            if (parameter is BackupJob job)
            {
                if (_backupJobService.DeleteJob(job))
                {
                    _backupViewModel.BackupJobs.Remove(job);
                    _backupViewModel.BackupJobs = new List<BackupJob>(_backupViewModel.BackupJobs);
                }
            }
        }

    }
}
