﻿using ConsoleTables;
using EasySave.Controller;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.CommandLine;
using System.Globalization;
using static EasySave.Model.Enum;
using Spectre.Console;

namespace EasySave;

class Program
{

    static IConfiguration BuildConfiguration(IConfigurationBuilder builder)
    {
        return builder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
    }

    private static BackupController _backupController;

    public static async Task<int> Main(string[] args)
    {

        var builder = new ConfigurationBuilder();
        var configuration = BuildConfiguration(builder);


        var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddConfiguration(configuration);
                })
                .ConfigureServices((context, services) =>
                {
                    new Startup(configuration).ConfigureServices(services);
                })
                .Build();



        _backupController = ActivatorUtilities.CreateInstance<BackupController>(host.Services);

        var culture = configuration["AppConfig:Language"];

        Resources.Translation.Culture = new CultureInfo(configuration["AppConfig:Language"]);

        #region ASCII Art
        string asciiart = @"
╔════════════════════════════════════════════════════════╗
║     ______                    _____                    ║
║    / ____/____ _ _____ __  __/ ___/ ____ _ _   __ ___  ║
║   / __/  / __ `// ___// / / /\__ \ / __ `/| | / // _ \ ║
║  / /___ / /_/ /(__  )/ /_/ /___/ // /_/ / | |/ //  __/ ║
║ /_____/ \__,_//____/ \__, //____/ \__,_/  |___/ \___/  ║
║                     /____/                             ║                     
╚════════════════════════════════════════════════════════╝";

        #endregion

        #region Options
        var jobName = new Option<string>(
            aliases: ["--name", "-n"],
                description: Resources.Translation.option_name)
        { IsRequired = true };

        var source = new Option<string>(
            aliases: ["--source", "-s"],
            description: Resources.Translation.option_source)
        { IsRequired = true };

        var dest = new Option<string>(
            aliases :["--dest","-d"],
            description: Resources.Translation.option_dest)
        { IsRequired = true };

        var type = new Option<JobTypeEnum>(
            aliases: ["--type", "-t"],
            description: Resources.Translation.option_type)
        { IsRequired = true };


        var id = new Option<string>(
            aliases: ["--job", "-j"],
            description: Resources.Translation.option_id)
        { IsRequired = true };

        var idNotRequired = new Option<string>(
            aliases: ["--job", "-j"],
            description: Resources.Translation.option_id);

        var all = new Option<bool>(
            aliases: ["--all", "-a"],
            description: "Pour choisir tous les travaux de sauvegarde");

        var lang = new Option<LanguageEnum>(
           aliases: ["--lang", "-l"],
           description: Resources.Translation.option_lang);

        #endregion

        #region Commands

        var rootCommand = new RootCommand(asciiart + "\n \n" + Resources.Translation.title);
        
        
        var createCommand = new Command("create", Resources.Translation.desc_create)
        {
                jobName,
                source,
                dest,
                type
        };
        var deleteCommand = new Command("delete", Resources.Translation.desc_delete)
        {
                id,
        };


        var showCommand = new Command("show", Resources.Translation.desc_show)
        {
                idNotRequired,
                all,
        };
        var runCommand = new Command("run", Resources.Translation.desc_run)
        {
                id,
        };
        var languageCommand = new Command("options", Resources.Translation.desc_options)
        {
                lang
        };

        rootCommand.AddCommand(runCommand);
        rootCommand.AddCommand(createCommand);
        rootCommand.AddCommand(showCommand);
        rootCommand.AddCommand(languageCommand);
        rootCommand.AddCommand(deleteCommand);

        #endregion

        #region Handlers

        runCommand.SetHandler((id) =>
        {
            OnRunJob(id);
        }, id);

        createCommand.SetHandler((name, source, dest, type) =>
        {
            OnCreateJob(name, source, dest, type);
        }, jobName, source, dest, type);

        showCommand.SetHandler((idNotRequired, all) =>
        {
            OnShowJob(idNotRequired, all);
        }, idNotRequired, all);

        languageCommand.SetHandler((lang) =>
        {
            OnChangeLanguage(lang);
        }, lang);

        deleteCommand.SetHandler((id) =>
        {
            OnDeleteJob(id);
        }, id);

        #endregion 


        if (args.Length == 0)
        {

            Console.WriteLine(Resources.Translation.welcome);

            await rootCommand.InvokeAsync("--help");

            while (true)
            {

                Console.Write("> ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                var result = await rootCommand.InvokeAsync(input.Split(' '));
                
            }
        }
        else
        {
            return await rootCommand.InvokeAsync(args);
        }
        return 0;


    }

    #region handlers methods
    private static void OnRunJob(string id)
    {
        _backupController.ExecuteJob(id);
    }

    private static void OnShowJob(string id , bool all)
    {

        var table = _backupController.ShowJob(id,all);
        AnsiConsole.Render(table);
    }


    private static void OnCreateJob(string jobName,string source, string dest, JobTypeEnum type)
    {
        _backupController.CreateJob(jobName, source, dest, type);
    }

    private static void OnChangeLanguage(LanguageEnum language)
    {
        _backupController.ChangeLanguage(language);
        Console.WriteLine(Resources.Translation.change_at_restart);
    }

    private static void OnDeleteJob(string idToDelete)
    {
        _backupController.DeleteJob(idToDelete);

    }
    #endregion
}
