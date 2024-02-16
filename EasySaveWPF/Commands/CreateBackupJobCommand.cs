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
    public class CreateBackupJobCommand : CommandBase
    {
        private readonly IBackupJobService _backupJobService;
        private readonly IStateLogService _stateLogService;

        private BackupJob _backupJob;

        public CreateBackupJobCommand(BackupJob backupJob, IBackupJobService backupJobService, IStateLogService stateLogService)
        {
            //_backupJobService = backupJobService;
            _backupJob = backupJob;
            _backupJobService = backupJobService;
            _stateLogService = stateLogService;
        }

        public override bool CanExecute(object? parameter)
        {
            return true;
        }

        public override async void Execute(object parameter)
        {
            if (_backupJobService.CreateJob(_backupJob))
            {
                _stateLogService.CreateStateLog(_backupJob);
            };
        }
    }
}
