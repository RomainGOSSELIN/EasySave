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
        private IStateLogService _stateLogService;
        private IDailyLogService _dailyLogService;


        public MainWindow(IBackupJobService backupJobService, IBackupService backupService,IStateLogService stateLogService, LoggerFactory loggerFactory, IDailyLogService dailyLogService)
        {
            _backupJobService = backupJobService;
            _backupService = backupService;
            _stateLogService = stateLogService;
            _dailyLogService = dailyLogService;
            InitializeComponent();


            DataContext = new MainViewModel(loggerFactory, _backupJobService, _backupService,_stateLogService,_dailyLogService);

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {

        }
    }
}