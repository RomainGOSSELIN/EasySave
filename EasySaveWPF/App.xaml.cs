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

namespace EasySaveWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;
        private ServiceProvider serviceProvider;
        public App()
        {
            ServiceCollection services = new ServiceCollection();

            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();

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
            const string appName = "EasySaveWPF"; 
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                
                Application.Current.Shutdown();
                MessageBox.Show("Une autre instance de l'application est en cours");
                return;
            }

            string langCode = EasySaveWPF.Resources.TranslationSettings.Default.LanguageCode;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(langCode);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCode);
            EasySaveWPF.Theme.AppTheme.ChangeTheme(new Uri("Theme/"+EasySaveWPF.Theme.Theme.Default.selectedTheme+".xaml", UriKind.Relative));
            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow.Show();

        }
    }
}
