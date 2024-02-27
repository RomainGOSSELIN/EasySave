using EasySaveWPF.Model;
using EasySaveWPF.Services.Interfaces;
using EasySaveWPF.ViewModel;


namespace EasySaveWPF.Commands
{
    public class DeleteJobCommand : CommandBase
    {
        private readonly IBackupJobService _backupJobService;
        private readonly IStateLogService _stateLogService;
        private BackupViewModel _backupViewModel;
        private static Notifications.Notifications notifications = new Notifications.Notifications();

        public DeleteJobCommand(IBackupJobService backupJobService, BackupViewModel vm)
        {
            _backupJobService = backupJobService;
            _backupViewModel = vm;
        }

        public override bool CanExecute(object? parameter)
        {
            if (_backupViewModel.BackupJobs.Find(j => j.State.State == Model.Enum.StateEnum.ACTIVE) != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void Execute(object parameter)
        {

            if (parameter is BackupJob job)
            {
                if (_backupJobService.DeleteJob(job))
                {
                    _backupViewModel.BackupJobs.Remove(job);
                    _backupViewModel.LoadBackupJobs();
                    //_backupViewModel.BackupJobs = new List<BackupJob>(_backupViewModel.BackupJobs);
                }
            }

        }

    }
}
