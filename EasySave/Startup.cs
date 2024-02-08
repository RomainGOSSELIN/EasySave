using EasySave.Controller;
using EasySave.Controller.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave
{
    internal class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IBackupController, BackupController>();
            services.AddSingleton<IBackupJobService, BackupJobService>();
            services.AddSingleton<IBackupService, BackupService>();
            services.AddSingleton<IDailyLogService, DailyLogService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IStateLogService, StateLogService>();
           
        }
    }
}
