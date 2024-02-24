using EasySaveWPF.Commands;
using EasySaveWPF.Model;
using EasySaveWPF.Services.Interfaces;
using System.Windows.Input;

namespace EasySaveWPF.ViewModel
{
    public class CreateBackupViewModel : ViewModelBase
    {

		public ICommand CreateCommand { get; set; }
		private BackupJob _backupJob;
		public BackupJob BackupJob
        {
			get
			{
				return _backupJob;
			}
			set
			{
                _backupJob = value;
				OnPropertyChanged(nameof(BackupJob));
			}
		}

	

		public CreateBackupViewModel(IBackupJobService backupJobService)
		{
			_backupJob = new BackupJob("","","",Model.Enum.JobTypeEnum.differential,0, new BackupState());
            CreateCommand = new CreateBackupJobCommand(this, backupJobService);
        }

      
    }
}
