using ConsoleTables;
using EasySave.Controller.Interfaces;
using EasySave.Model;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using static EasySave.Model.Enum;
using Spectre.Console;

namespace EasySave.Controller
{
    public class BackupController : IBackupController
    {
        private static IBackupJobService _backupJobService;
        private static IBackupService _backupService;
        private static IStateLogService _stateLogService;
        private static IDailyLogService _dailyLogService ;
        private static ISettingsService _settingsService;
        private static IConfiguration _configuration;

        public BackupController(IBackupJobService backupJobService, IBackupService backupService,
                                IStateLogService stateLogService, IDailyLogService dailyLogService,
                                ISettingsService settingsService, IConfiguration configuration)
        {
            _configuration = configuration;
            _backupJobService = backupJobService;
            _backupService = backupService;
            _stateLogService = stateLogService;
            _dailyLogService = dailyLogService;
            _settingsService = settingsService;
        }

        public void ExecuteJob(string id)
        {
            var separators = new Char [] { ':', ';' };
            List<BackupJob> backupJobs = [];

            if (separators.Any(id.Contains)) {
                backupJobs = _backupJobService.GetJobs(id.Split(separators).Select(int.Parse).ToList());
            }
            else {
                // Check if the id is a number
                if (!int.TryParse(id, out _))
                {
                    Console.WriteLine("L'ID doit être un nombre.");
                    return;
                }
                backupJobs.Add(_backupJobService.GetJob(int.Parse(id)));
            };
            foreach (var backupJob in backupJobs)
            {
                if (backupJob != null)
                {
                     var stopwatch = new Stopwatch();
                    var FileSize = GetDirectorySize(backupJob.SourceDir);

                    stopwatch.Start();
                    _backupService.ExecuteBackupJob(backupJob);
                    stopwatch.Stop();
                    _dailyLogService.AddDailyLog(backupJob, FileSize, (int)stopwatch.ElapsedMilliseconds);
                }
                else 
                {
                    Console.WriteLine("Aucun travail trouvé");
                    return;
                }
               
            }

        }

        public void CreateJob(string jobName, string source, string dest , JobTypeEnum type)
        {
            BackupJob backupJob = new BackupJob(jobName, source, dest, type);
            if (_backupJobService.CreateJob(backupJob))
            {
                _stateLogService.CreateStateLog(backupJob);
            };
        }
        public void DeleteJob(string idToDelete)
        {
            if (!int.TryParse(idToDelete, out _))
            {
                Console.WriteLine("L'ID doit être un nombre.");
                return;
            }

            int intIdToDelete = Int32.Parse(idToDelete);
            if (_backupJobService.DeleteJob(intIdToDelete))
            {
                _stateLogService.DeleteStateLog(intIdToDelete);
            }
        }
        public long GetDirectorySize(string path)
        {
            return Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                .Select(file => new FileInfo(file).Length)
                .Sum();
        }

        public Table ShowJob(string id, bool all)
        {
            List<BackupJob> jobs = new List<BackupJob>();
            if (all)
            {
                jobs = _backupJobService.GetAllJobs();
            }
            else
            {
                var ids = ParseInputString(id);
                jobs = _backupJobService.GetJobs(ids);
            }
            var table = new Table();
            table.AddColumn(new TableColumn("ID").RightAligned().Width(2).LeftAligned());
            table.AddColumn(new TableColumn("Nom").Width(20).LeftAligned());
            table.AddColumn(new TableColumn("Source").Width(40).LeftAligned());
            table.AddColumn(new TableColumn("Destination").Width(40).LeftAligned());
            table.AddColumn(new TableColumn("Type").Width(12).LeftAligned());
            if (jobs == null || jobs.Count == 0)
            {
                Console.WriteLine(Resources.Translation.no_job_found);
            }
            else
            {
                foreach (var job in jobs)
                {
                    table.AddRow(job.Id.ToString(), job.Name.ToString(), job.SourceDir.ToString(), job.TargetDir.ToString(), job.Type.ToString());
                    table.AddEmptyRow();
                }
            }
            return table;
        }




        static List<int> ParseInputString(string input)
        {
            string[] parts = input.Split(':', ';');
            List<int> ids = new List<int>();
            List<int> result = new List<int>();

            foreach (string part in parts)
            {
                if (int.TryParse(part, out int id))
                {
                    ids.Add(id);
                }
            }

            if (input.Contains(':'))
            {
                for (int i = ids[0]; i <= ids[1]; i++)
                {
                    result.Add(i);
                }
            }

            else
            {
                foreach (int i in ids)
                {
                    result.Add(i);
                }

            }
            return result;
        }

        public void ChangeLanguage(LanguageEnum language)
        {
            _settingsService.ChangeLanguage(language);
        }


    }

    

}
