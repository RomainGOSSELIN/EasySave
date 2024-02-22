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

        private ObservableCollection<BackupJob> _backupJobs;
        public IEnumerable<BackupJob> BackupJobs => _backupJobs;

        public ICommand DeleteCommand { get; set; }
        public ICommand RunCommand { get; set; }
        public ICommand RunAllCommand { get; set; }
        public ICommand RunSomeCommand { get; set; }

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

        public BackupViewModel(LoggerFactory loggerFactory, IBackupJobService backupJobService, IBackupService backupService, IStateLogService stateLogService, IDailyLogService dailyLogService)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger(Model.Enum.LogType.Json);
            _backupJobService = backupJobService;
            _backupService = backupService;
            _backupService.CurrentBackupStateChanged += BackupService_CurrentStateChanged;
            _stateLogService = stateLogService;
            _dailyLogService = dailyLogService;

            _backupJobs = new ObservableCollection<BackupJob>(backupJobService.GetAllJobs());
            DeleteCommand = new DeleteJobCommand(_backupJobService, _backupJobs, _stateLogService);
            RunCommand = new RunJobCommand(_backupService, _dailyLogService);
            RunAllCommand = new RunAllJobCommand(_backupService, _backupJobs, _dailyLogService);
            RunSomeCommand = new RunSomeJobCommand(backupService, _backupJobs, _dailyLogService , this);
        }

        private void BackupService_CurrentStateChanged(object? sender, BackupState e)
        {
            CurrentStateBackup = e;
            FileSizeProgress = _currentStateBackup.TotalFilesSize - _currentStateBackup.NbFilesSizeLeftToDo;
            FileProgress = _currentStateBackup.TotalFilesToCopy - _currentStateBackup.NbFilesLeftToDo;
            if (_currentStateBackup.NbFilesLeftToDo == 0)
            {
                FileSizeProgress = 0;
                FileProgress = 0;
                CurrentStateBackup = null;
            }
        }
    }
}
