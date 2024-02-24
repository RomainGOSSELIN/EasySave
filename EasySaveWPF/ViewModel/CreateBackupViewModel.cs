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

		private MainViewModel _mainViewModel;
		

		public CreateBackupViewModel(IBackupJobService backupJobService, MainViewModel vm)
		{
            _backupJob = new BackupJob("","","",Model.Enum.JobTypeEnum.differential,0, new BackupState());
            _mainViewModel = vm;

            CreateCommand = new CreateBackupJobCommand(this, _mainViewModel, backupJobService);
        }

      
    }
}
