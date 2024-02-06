using ConsoleTables;
using EasySave.ViewModel;
using System.CommandLine;
using System.Globalization;
using static EasySave.Model.Enum;

namespace EasySave;

class Program
{
    private static BackupViewModel _viewModel = new BackupViewModel();

    public static async Task<int> Main(string[] args)
    {
        Resources.Translation.Culture = new CultureInfo("es");
        
        string asciiart = @"
####################################################
#     ______                 _____                 #
#    / ____/___ ________  __/ ___/____ __   _____  #
#   / __/ / __ `/ ___/ / / /\__ \/ __ `/ | / / _ \ #
#  / /___/ /_/ (__  ) /_/ /___/ / /_/ /| |/ /  __/ #
# /_____/\__,_/____/\__, //____/\__,_/ |___/\___/  #
#                  /____/                          #                      
####################################################";


        //Console.WriteLine(asciiart);


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



        var lang = new Option<LanguageEnum>(
           aliases: ["--lang", "-l"],
           description: Resources.Translation.option_lang);



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
                id,
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




        runCommand.SetHandler((id) =>
        {
            OnRunJob(id);
        }, id);

        createCommand.SetHandler((name, source, dest, type) =>
        {
            OnCreateJob(name, source, dest, type);
        }, jobName, source, dest, type);

        showCommand.SetHandler((id) =>
        {
            OnShowJob(id);
        }, id);

        languageCommand.SetHandler((lang) =>
        {
            OnChangeLanguage(lang);
        }, lang);

        deleteCommand.SetHandler((id) =>
        {
            OnDeleteJob(id);
        }, id);


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


    private static void OnRunJob(string id)
    {
        _viewModel.ExecuteJob(id);
    }

    private static void OnShowJob(string id)
    {

        var table = _viewModel.ShowJob(id);
        table.Write();

    }


    private static void OnCreateJob(string jobName,string source, string dest, JobTypeEnum type)
    {
        _viewModel.CreateJob(jobName, source, dest, type);
    } 

    private static void OnChangeLanguage(LanguageEnum language)
    {
        Console.WriteLine($"La Langue est {language}");
    }

    private static void OnDeleteJob(string idToDelete)
    {
        _viewModel.DeleteJob(idToDelete);

    }

}
