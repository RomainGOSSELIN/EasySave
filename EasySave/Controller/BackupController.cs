using ConsoleTables;
using EasySave.Controller.Interfaces;
using EasySave.Model;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using static EasySave.Model.Enum;
using Spectre.Console;
using System.Linq;

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
                var ids = ParseInputString(id);
                backupJobs = _backupJobService.GetJobs(ids);
            }

            else {
                // Check if the id is a number
                if (!int.TryParse(id, out _))
                {
                    Console.WriteLine(Resources.Translation.id_must_number);
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
                    Console.WriteLine(Resources.Translation.no_job_found);
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
                Console.WriteLine(Resources.Translation.id_must_number);
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
            List<BackupJob> jobs;

            if (all)
            {
                jobs = _backupJobService.GetAllJobs();
            }
            else
            {
                var ids = ParseInputString(id);
                jobs = _backupJobService.GetJobs(ids);
            }

            if (jobs == null)
            {
                Console.WriteLine(Resources.Translation.no_job_found);
                return null;
            }

            var table = new Table();
            table.AddColumn(new TableColumn(Resources.Translation.number).RightAligned().Width(2).LeftAligned());
            table.AddColumn(new TableColumn(Resources.Translation.job_name).Width(20).LeftAligned());
            table.AddColumn(new TableColumn(Resources.Translation.source).Width(40).LeftAligned());
            table.AddColumn(new TableColumn(Resources.Translation.destination).Width(40).LeftAligned());
            table.AddColumn(new TableColumn(Resources.Translation.type).Width(12).LeftAligned());

            foreach (var job in jobs)
            {
                if (job != null)
                {
                    table.AddRow(
                        job.Id?.ToString() ?? "",
                        job.Name ?? "",
                        job.SourceDir ?? "",
                        job.TargetDir ?? "",
                        job.Type.ToString() ?? ""
                    );
                    table.AddEmptyRow();
                }
            }

            return table;
        }


        static List<int> ParseInputString(string input)
        {
            List<int> result = new List<int>();
            List<int> ids = new List<int>();
            if (input != null)
            {
            string[] parts = input.Split(':', ';');

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
            }
            return result;
        }

        public void ChangeOptions(LanguageEnum language, LogTypeEnum logType)
        {
            _settingsService.ChangeOptions(language, logType);
        }


    }

    

}
