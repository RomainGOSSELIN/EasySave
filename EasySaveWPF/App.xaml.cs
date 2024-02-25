using EasySaveWPF.Model.LogFactory;
using EasySaveWPF.Services;
using EasySaveWPF.Services.Interfaces;
using EasySaveWPF.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Windows;
using System.Text.Json; 
using System.Threading;

namespace EasySaveWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;
        public ServerConsole server;
        
        public App()
        {
            ServiceCollection services = new ServiceCollection();

            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
            StateLogService stateLogService = new StateLogService();
            server = new ServerConsole(stateLogService);

        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<LoggerFactory>();
            services.AddSingleton<IDailyLogService, DailyLogService>();
            services.AddSingleton<IBackupJobService, BackupJobService>();
            services.AddSingleton<IBackupService, BackupService>();
            services.AddSingleton<IStateLogService, StateLogService>();
            services.AddSingleton<BackupViewModel>();
            services.AddSingleton<MainWindow>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            
            string langCode = EasySaveWPF.Resources.TranslationSettings.Default.LanguageCode;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(langCode);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCode);
            EasySaveWPF.Theme.AppTheme.ChangeTheme(new Uri("Theme/"+EasySaveWPF.Theme.Theme.Default.selectedTheme+".xaml", UriKind.Relative));
            var mainWindow = serviceProvider.GetService<MainWindow>();
            
            server.Start();
            mainWindow.Show();

        }
        

    }

}

