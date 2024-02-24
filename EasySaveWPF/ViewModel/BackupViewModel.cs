using EasySaveWPF.Model;
using EasySaveWPF.Model.LogFactory;

using EasySaveWPF.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;
using EasySaveWPF.Core;
using EasySaveWPF.Commands;
namespace EasySaveWPF.ViewModel
{
    public class BackupViewModel : ViewModelBase
    {
        private LoggerFactory _loggerFactory;
        private IBackupJobService _backupJobService;
        private IBackupService _backupService;
        private IStateLogService _stateLogService;
        private IDailyLogService _dailyLogService;
        private ILogger _logger;
        private List<BackupJob> _backupJobs;
        private BackupJob _selectedJobBeforeUpdate;
        private static readonly object _lock = new object();


        public List<BackupJob> BackupJobs
        {
            get
            {
                return _backupJobs;
            }
            set
            {
                _backupJobs = (List<BackupJob>)value;
                OnPropertyChanged(nameof(BackupJobs));
            }
        }

        #region Commands
        public ICommand DeleteCommand { get; set; }
  
        public ICommand RunFactoCommand {  get; set; }
        public ICommand PauseCommand { get; set; }
        #endregion

        #region Propchanges
        private BackupState _currentStateBackup;
        public BackupState CurrentStateBackup
        {
            get { return _currentStateBackup; }
            set
            {
                _currentStateBackup = value;
                OnPropertyChanged(nameof(CurrentStateBackup));
            }
        }

        private long _fileSizeProgress;
        public long FileSizeProgress
        {
            get
            {
                return _fileSizeProgress;
            }
            set
            {
                _fileSizeProgress = value;
                OnPropertyChanged(nameof(FileSizeProgress));
            }
        }

        private long _fileProgress;
        public long FileProgress
        {
            get
            {
                return _fileProgress;
            }
            set
            {
                _fileProgress = value;
                OnPropertyChanged(nameof(FileProgress));
            }
        }

        private int _toJob;
        public int ToJob
        {
            get
            {
                return _toJob;
            }
            set
            {
                _toJob = value;
                OnPropertyChanged(nameof(ToJob));
            }
        }

        private int _fromJob;
        public int FromJob
        {
            get
            {
                return _fromJob;
            }
            set
            {
                _fromJob = value;
                OnPropertyChanged(nameof(FromJob));
            }
        }

        private string _runOperation;
        public string RunOperation
        {
            get
            {
                return _runOperation;
            }
            set
            {
                _runOperation = value;
                OnPropertyChanged(nameof(RunOperation));
            }
        }

        private BackupJob _selectedJob;
        public BackupJob  SelectedJob
        {
            get
            {
                return _selectedJob;
            }
            set
            {
                _selectedJob = value == null ? BackupJobs.Find(x => x.Id == _selectedJobBeforeUpdate.Id) : value;
                OnPropertyChanged(nameof(SelectedJob));
            }
        }
        #endregion

        public BackupViewModel(LoggerFactory loggerFactory, IBackupJobService backupJobService, IBackupService backupService, IStateLogService stateLogService, IDailyLogService dailyLogService)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger(Model.Enum.LogType.Json);
            _backupJobService = backupJobService;
            _backupService = backupService;
            _backupService.CurrentBackupStateChanged += BackupService_CurrentStateChanged;
            _stateLogService = stateLogService;
            _dailyLogService = dailyLogService;

            _backupJobs = new List<BackupJob>(backupJobService.GetAllJobs());
            DeleteCommand = new DeleteJobCommand(_backupJobService, _backupJobs, _stateLogService);
         
            RunFactoCommand = new RunFactoCommand(_backupService, _dailyLogService, this);
            PauseCommand = new PauseCommand();
        }

        private void BackupService_CurrentStateChanged(object? sender, BackupJob e)
        {

            var job = e;
            lock (_lock)
            {
                // Recherchez l'index du job dans la liste
                int index = BackupJobs.FindIndex(j => j.Id == e.Id);
                if (index != -1)
                {
                    // Créez une copie du job pour éviter de modifier l'objet d'origine
                    BackupJob updatedJob = new BackupJob();
                    updatedJob = e;
                    // Mettez à jour l'état du job dans la copie
                    BackupJobs[index].State = updatedJob.State;
                    // Affectez la liste mise à jour à la propriété BackupJobs
                    BackupJobs = new List<BackupJob>(BackupJobs);
                }
            }

        }

        public void LoadBackupJobs()
        {
            _selectedJobBeforeUpdate = SelectedJob;
            BackupJobs = new List<BackupJob>(_backupJobService.GetAllJobs());

        }
    }
}
