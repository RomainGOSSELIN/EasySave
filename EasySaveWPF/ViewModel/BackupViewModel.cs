using EasySaveWPF.Model;
using EasySaveWPF.Model.LogFactory;

using EasySaveWPF.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;
using EasySaveWPF.Core;
using EasySaveWPF.Commands;
using System.Diagnostics;
using System.IO;
using EasySaveWPF.Services;
using System.Windows;
using static EasySaveWPF.Services.ServerService;
namespace EasySaveWPF.ViewModel
{
    public class BackupViewModel : ViewModelBase
    {
        private LoggerContext _loggerStrategy;
        private IBackupJobService _backupJobService;
        private IBackupService _backupService;
        private IServerService _serverService;
        private IDailyLogService _dailyLogService;
        private List<BackupJob> _backupJobs;
        private BackupJob _selectedJobBeforeUpdate;
        private static readonly object _lock = new object();
        private static string _processName = Path.GetFileNameWithoutExtension(Properties.Settings.Default.BusinessSoftwarePath);
        private Thread checkBusinessSoftwareThread;
        private static CancellationTokenSource _businessCancellationToken = new CancellationTokenSource();

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

        public ICommand RunFactoCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand StopCommand { get; set; }
        #endregion

        #region Propchanges

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
                _fromJob = (int)value;
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
        public BackupJob SelectedJob
        {
            get
            {
                return _selectedJob;
            }
            set
            {
                _selectedJob = value == null ? BackupJobs.FirstOrDefault()  : value;
                _selectedJobBeforeUpdate = _selectedJob;

                OnPropertyChanged(nameof(SelectedJob));
            }
        }
        #endregion

        public BackupViewModel(LoggerContext loggerStrategy, IBackupJobService backupJobService, IBackupService backupService, IDailyLogService dailyLogService, IServerService serverService)
        {
            #region Init
            _loggerStrategy = loggerStrategy;
            _loggerStrategy.SetStrategy(new JsonService());
            _backupJobService = backupJobService;
            _backupService = backupService;
            _backupService.CurrentBackupStateChanged += BackupService_CurrentStateChanged;
            _dailyLogService = dailyLogService;
            _serverService = serverService;

            RunOperation = "to";
            DeleteCommand = new DeleteJobCommand(_backupJobService, this);
            RunFactoCommand = new RunFactoCommand(_backupService, _dailyLogService, this);
            PauseCommand = new PauseCommand(this);
            StopCommand = new StopCommand();
            #endregion

            _serverService.Start();
            _serverService.DataReceived += HandleDataReceived;
            LoadBackupJobs();

            //Lancement thread de vérif de l'état du logiciel métier :

            checkBusinessSoftwareThread = new Thread(new ThreadStart(CheckBusinessSoftwareState));
            checkBusinessSoftwareThread.Start();



        }

        private void BackupService_CurrentStateChanged(object? sender, BackupJob e)
        {

            var job = e;
            lock (_lock)
            {
                int index = BackupJobs.FindIndex(j => j.Id == e.Id);
                if (index != -1)
                {
                    BackupJob updatedJob = new BackupJob();
                    updatedJob = e;
                    BackupJobs[index].State = updatedJob.State;
                    BackupJobs = new List<BackupJob>(BackupJobs);
                }
                _serverService.SendDataToClients(BackupJobs);

            }
            SelectedJob = _selectedJobBeforeUpdate;

        }

        private void HandleDataReceived(object sender, CommandWithParameter e)
        {
            switch (e.Command)
            {
                case "stop":
                    StopCommand.Execute(BackupJobs.Single(x => x.Id == (e.Parameter)));
                    break;
                case "run":
                    RunFactoCommand.Execute(BackupJobs.Single(x => x.Id == (e.Parameter)));
                    break;
                case "pause":

                    PauseCommand.Execute(BackupJobs.Single(x => x.Id == ((e.Parameter))));
                    break;
                default:
                    break;
            }


        }

        public void LoadBackupJobs()
        {
            BackupJobs = new List<BackupJob>(_backupJobService.GetAllJobs());
            _selectedJobBeforeUpdate = BackupJobs.FirstOrDefault();
            _serverService.SendDataToClients(BackupJobs);

        }

        public void StopBackupJobs()
        {
            foreach (BackupJob job in BackupJobs)
            {
                if (job.State.State == Model.Enum.StateEnum.ACTIVE)
                {
                    job.State.State = Model.Enum.StateEnum.END;
                    job.CancellationTokenSource.Cancel();

                }
                job.State = new BackupState();
                _backupJobService.UpdateJob(job);
            }
        }

        public void StopBusinessSoftwareStateCheck()
        {
            _businessCancellationToken.Cancel();
        }
        public void CheckBusinessSoftwareState()
        {

            bool isProcessRunning = false;

            while (!_businessCancellationToken.IsCancellationRequested)
            {
                Process[] processes = Process.GetProcessesByName(_processName);

                if (processes.Length > 0)
                {
                    if (!isProcessRunning)
                    {

                        isProcessRunning = true;
                        foreach (BackupJob job in BackupJobs)
                        {
                            if (job.State.State == Model.Enum.StateEnum.ACTIVE)
                            {
                                job.ResetEvent.Reset();
                                job.State.State = Model.Enum.StateEnum.PAUSED;
                            }
                        }
                    }
                }
                else
                {
                    if (isProcessRunning)
                    {

                        isProcessRunning = false;
                        foreach (BackupJob job in BackupJobs)
                        {
                            if (job.State.State == Model.Enum.StateEnum.PAUSED)
                            {
                                job.ResetEvent.Set();
                                job.State.State = Model.Enum.StateEnum.ACTIVE;
                            }
                        }
                    }
                }

                Thread.Sleep(1000);
            }

        }
    }
}
