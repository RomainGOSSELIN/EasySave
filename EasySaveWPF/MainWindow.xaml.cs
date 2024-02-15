using EasySaveWPF.Model.LogFactory;
using EasySaveWPF.Services.Interfaces;
using EasySaveWPF.ViewModel;
using Microsoft.Extensions.DependencyInjection;
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

        public MainWindow(IBackupJobService backupJobService, IBackupService backupService, LoggerFactory loggerFactory)
        {
            _backupJobService = backupJobService;
            _backupService = backupService;

            InitializeComponent();


            DataContext = new JobListingViewModel(loggerFactory, _backupJobService, _backupService);

        }
    }
}