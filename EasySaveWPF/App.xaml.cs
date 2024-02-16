using EasySaveWPF.Model.LogFactory;
using EasySaveWPF.Services;
using EasySaveWPF.Services.Interfaces;
using EasySaveWPF.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Configuration;
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
        private ServiceProvider serviceProvider;
        private IConfiguration _configuration;
        public App()
        {
            ServiceCollection services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(_configuration);
            services.AddSingleton<LoggerFactory>();
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
            mainWindow.Show();
        }
    }
}
