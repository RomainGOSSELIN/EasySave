using ClientWPFConsole.Commands;
using ClientWPFConsole.Model;
using ClientWPFConsole.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace ClientWPFConsole.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        public ICommand RunJobCommand { get; set; }
        public ICommand PauseJobCommand { get; set; }
        public ICommand ConnectToServer { get; set; }
        public ICommand StopJobCommand { get; set; }

        public ClientService  ClientService;

        private ObservableCollection<BackupJob> _backupJobs;
        public ObservableCollection<BackupJob> BackupJobs
        {
            get { return _backupJobs; }
            set
            {
                _backupJobs = value;
                OnPropertyChanged(nameof(BackupJobs));
            }
        }
        
        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
            }
        }

        private string _serverIp;
        public string ServerIp
        {
            get
            {
                return _serverIp;
            }
            set
            {
                _serverIp = value;
                OnPropertyChanged(nameof(ServerIp));
            }
        }


        public MainViewModel()
        {
            RunJobCommand = new RunJobCommand(this);
            StopJobCommand = new StopJobCommand(this);
            PauseJobCommand = new PauseJobCommand(this);
            ConnectToServer = new ConnectToServerCommand(this);
            ServerIp = "";
            BackupJobs = new ObservableCollection<BackupJob>();
            ClientService = new ClientService();
            ClientService.DataReceived += ClientService_DataReceived;
            ClientService.StatusChanged += ClientService_StatusChanged;

        }

        public void ClientService_DataReceived(object sender, List<BackupJob> backupJobs)
        {

            App.Current.Dispatcher.Invoke(() =>
            {
                BackupJobs.Clear();
                foreach (var job in backupJobs)
                {
                    BackupJobs.Add(job);
                }
            });
        }

        public void ClientService_StatusChanged(object sender, bool isConnected)
        {
            IsConnected = isConnected;
        }



    }
}
