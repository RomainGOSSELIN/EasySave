﻿using EasySaveWPF.Model.LogFactory;
using EasySaveWPF.Services;
using EasySaveWPF.Services.Interfaces;
using EasySaveWPF.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasySaveWPF
{
    public partial class MainWindow : Window
    {
        private IBackupJobService _backupJobService;
        private IBackupService _backupService;
        private IDailyLogService _dailyLogService;
        private IServerService _serverService;

        public MainWindow(IBackupJobService backupJobService, IBackupService backupService, LoggerContext loggerContext, IDailyLogService dailyLogService, IServerService serverService)
        {
            _backupJobService = backupJobService;
            _backupService = backupService;
            _dailyLogService = dailyLogService;
            _serverService = serverService;
            InitializeComponent();


            DataContext = new MainViewModel(loggerContext, _backupJobService, _backupService,_dailyLogService, _serverService);

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        public void BackupView_Checked(object sender)
        {
            AddButton.IsChecked = true;
        }

        private void OpenLinkButton_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/RomainGOSSELIN/EasySave/wiki/Accueil";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}