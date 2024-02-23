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
    public class DeleteJobCommand : CommandBase
    {
        private readonly IBackupJobService _backupJobService;
        private readonly IStateLogService _stateLogService;

        private readonly List<BackupJob> _backupJobs;

        public DeleteJobCommand(IBackupJobService backupJobService, List<BackupJob> backupJobs, IStateLogService stateLogService)
        {
            _backupJobService = backupJobService;
            _backupJobs = backupJobs;
            _stateLogService = stateLogService;
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
                    _backupJobs.Remove(job);
                    _stateLogService.DeleteStateLog(job);
                }
            }
        }

    }
}
