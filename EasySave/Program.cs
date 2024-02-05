using EasySave.Model;
using EasySave.ViewModel;
using System;
using System.CommandLine;
using System.IO;
using static EasySave.Model.Enum;

namespace EasySave;

class Program
{
    private static BackupJobService backupJobService = new BackupJobService();
    private static BackupService backupService = new BackupService();

    public static async Task<int> Main(string[] args)
    {



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
                name: "--name",
                description: "Le nom du travil de sauvegarde")
        { IsRequired = true };

        var source = new Option<string>(
            name: "--source",
            description: "La source du répertoire du travail de sauvegarde")
        { IsRequired = true };

        var dest = new Option<string>(
            name: "--dest",
            description: "La destination du répertoire du travil de sauvegarde")
        { IsRequired = true };

        var type = new Option<JobTypeEnum>(
            name: "--type",
            description: "Pour choisir un travil de sauvegarde complet")
        { IsRequired = true };


        var id = new Option<int>(
            name: "--id",
            description: "L'id du travail de sauvegarde")
        { IsRequired = true };



        var lang = new Option<Languages>(
           name: "-l",
           description: "Pour choisir la langue en français");



        var rootCommand = new RootCommand(asciiart +"\n \n Application de sauvegarde de fichiers");
        
 
        var createCommand = new Command("create", "Créer un nouveau travail de sauvegarde")
        {
                jobName,
                source,
                dest,
                type
        };
        var deleteCommand = new Command("delete", "Supprimer un travail de sauvegarde")
        {
                id,
        };
        var showCommand = new Command("show", "Afficher un travail de sauvegarde")
        {
                id,
        };
        var runCommand = new Command("run", "Lancer un travail de sauvegarde")
        {
                id,
        };
        var languageCommand = new Command("options", "Régler les options comme la langue")
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


        return await rootCommand.InvokeAsync(args);

    }


    private static void OnRunJob(int id)
    {

        Console.WriteLine($"Le travail a lancer est: {id}");
        backupService.ExecuteBackupJob(backupJobService.GetJob(id));
    }

    private static void OnShowJob(int id)
    {
        Console.WriteLine($"Le travail a montrer est: {id}");
    }


    private static void OnCreateJob(string jobName,string source, string dest, JobTypeEnum type)
    {
        BackupJob backupJob = new BackupJob(jobName, source, dest, type);

        backupJobService.CreateJob(backupJob);
    }

    private static void OnChangeLanguage(Languages language)
    {
        Console.WriteLine($"La Langue est language");
    }

    private static void OnDeleteJob(int idToDelete)
    {
        backupJobService.DeleteJob(idToDelete);
    }



}