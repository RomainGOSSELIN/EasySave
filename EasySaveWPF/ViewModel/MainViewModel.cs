using System;
using System.ComponentModel;
using System.Windows;
using EasySaveWPF.Core;
using EasySaveWPF.Model.LogFactory;
using EasySaveWPF.Resources;
using EasySaveWPF.Services.Interfaces;
using static EasySaveWPF.Model.Enum;


namespace EasySaveWPF.ViewModel
{
    public class MainViewModel : ObservableObject
    {

        public RelayCommand BackupViewCommand { get; set; }
        public RelayCommand SettingsViewCommand { get; set; }
        public RelayCommand CreateBackupViewCommand { get; set; }
        public BackupViewModel BackupVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }
        public CreateBackupViewModel CreateBackupVM { get; set; }
        Notifications.Notifications notifications = new Notifications.Notifications();

        private object _currentview;
        public object CurrentView
        {
            get { return _currentview; }
            set
            {
                _currentview = value;
                OnPropertyChanged();
            }

        }

        private bool _backupButton;
        public bool BackupViewButton
        {
            get
            {
                return _backupButton;
            }
            set
            {
                _backupButton = value;
                OnPropertyChanged(nameof(BackupViewButton));
            }
        }

        public MainViewModel(LoggerContext loggerStrategy, IBackupJobService backupJobService, IBackupService backupService, IDailyLogService dailyLogService, IServerService serverService)
        {
            App.Current.MainWindow.Closing += new CancelEventHandler(OnWindowClosing);


            BackupVM = new BackupViewModel(loggerStrategy, backupJobService, backupService, dailyLogService, serverService);
            SettingsVM = new SettingsViewModel();
            CreateBackupVM = new CreateBackupViewModel(backupJobService, this);

            CurrentView = BackupVM;
            BackupViewButton = true;

            BackupViewCommand = new RelayCommand(o =>
            {
                BackupVM.LoadBackupJobs();
                BackupViewButton = true;
                CurrentView = BackupVM;

            });

            SettingsViewCommand = new RelayCommand(o =>
            {
                CurrentView = SettingsVM;
            });

            CreateBackupViewCommand = new RelayCommand(o =>
            {

                CurrentView = CreateBackupVM;
            });
        }

        // A la fermeture on stop les jobs actifs et le thread du check du logiciel métier
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (BackupVM.BackupJobs.Any(job => job.State.State == StateEnum.ACTIVE))
            {
                var result = notifications.StopBeforeClosing();
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    BackupVM.StopBackupJobs();
                }
            }
            

        }

    }
}
