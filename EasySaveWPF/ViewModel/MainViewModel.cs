using System;
using EasySaveWPF.Core;
using EasySaveWPF.Model.LogFactory;
using EasySaveWPF.Services.Interfaces;


namespace EasySaveWPF.ViewModel
{
    class MainViewModel : ObservableObject
    {

        public RelayCommand BackupViewCommand { get; set; }
        public RelayCommand SettingsViewCommand { get; set; }
        public RelayCommand CreateBackupViewCommand { get; set; }


        public BackupViewModel BackupVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }
        public CreateBackupViewModel CreateBackupVM { get; set; }

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

        public MainViewModel(LoggerFactory loggerFactory,IBackupJobService backupJobService,IBackupService backupService, IStateLogService stateLogService)
        {
            BackupVM = new BackupViewModel(loggerFactory, backupJobService, backupService, stateLogService);
            SettingsVM = new SettingsViewModel();

            CurrentView = BackupVM;

            BackupViewCommand = new RelayCommand(o =>
            {
                BackupVM = new BackupViewModel(loggerFactory, backupJobService, backupService,stateLogService);
                CurrentView = BackupVM;
            });

            SettingsViewCommand = new RelayCommand(o =>
            {
                CurrentView = SettingsVM;
            });

            CreateBackupViewCommand = new RelayCommand(o =>
            {
                CreateBackupVM = new CreateBackupViewModel(backupJobService, stateLogService);

                CurrentView = CreateBackupVM;
            });
        }
    }
}
