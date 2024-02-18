using EasySaveWPF.Model;
using EasySaveWPF.Services.Interfaces;
using EasySaveWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasySaveWPF.Commands
{
    public class CreateBackupJobCommand : CommandBase
    {
        private readonly IBackupJobService _backupJobService;
        private readonly IStateLogService _stateLogService;

        private CreateBackupViewModel _viewModel;

        private BackupJob _backupJob => _viewModel.BackupJob;

        public CreateBackupJobCommand(CreateBackupViewModel viewModel, IBackupJobService backupJobService, IStateLogService stateLogService)
        {
            //_backupJobService = backupJobService;
            _viewModel = viewModel;
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

                _viewModel.BackupJob = new BackupJob("","","",Model.Enum.JobTypeEnum.differential);
            };

        }
    }
}
