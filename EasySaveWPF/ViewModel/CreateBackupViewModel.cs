using EasySaveWPF.Commands;
using EasySaveWPF.Model;
using EasySaveWPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace EasySaveWPF.ViewModel
{
    class CreateBackupViewModel : ViewModelBase
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

		private int _backupJobName;
		public int BackupJobName
        {
			get
			{
				return _backupJobName;
			}
			set
			{
				_backupJobName = value;
				OnPropertyChanged(nameof(BackupJobName));
			}
		}

		public CreateBackupViewModel(IBackupJobService backupJobService, IStateLogService stateLogService)
		{
			_backupJob = new BackupJob("","","",Model.Enum.JobTypeEnum.full,0);
            CreateCommand = new CreateBackupJobCommand(_backupJob, backupJobService, stateLogService);

        }

      
    }
}
