﻿using ConsoleTables;
using EasySave.Controller;
using EasySave.Controller.Interfaces;
using EasySave.Model;
using Microsoft.Extensions.Configuration;
using static EasySave.Model.Enum;

namespace EasySave.ViewModel
{
    public class BackupController : IBackupController
    {
        private static IBackupJobService _backupJobService;
        private static IBackupService _backupService;
        private static IStateLogService _stateLogService;

        private static IConfiguration _configuration;
        public BackupController(IConfiguration configuration)
        {
            _configuration = configuration;
            _backupJobService = new BackupJobService(_configuration);
            _backupService = new BackupService();
            _stateLogService = new StateLogService(_configuration);

        }

        public void ExecuteJob(string id)
        {
            var separators = new Char [] { ':', ';' };
            List<BackupJob> backupJobs = [];

            if (separators.Any(id.Contains)) {
                backupJobs = _backupJobService.GetJobs(id.Split(separators).Select(int.Parse).ToList());
            }
            else {
                backupJobs.Add(_backupJobService.GetJob(int.Parse(id)));
            };

            foreach (var backupJob in backupJobs)
            {

                _backupService.ExecuteBackupJob(backupJob);

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
            int intIdToDelete = Int32.Parse(idToDelete);
            if (_backupJobService.DeleteJob(intIdToDelete))
            {
                _stateLogService.DeleteStateLog(intIdToDelete);
            }
        }

        public ConsoleTable ShowJob(string id, bool all)
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
           

            var table = new ConsoleTable("Numero", "Nom du Travail", "Source", "Destination", "Type");
            if(jobs == null)
            {
                Console.WriteLine("Aucun travail trouvé");
            }
            else
            {

            foreach (var job in jobs)
            {
                if (job != null)
                {
                    table.AddRow(job.Id, job.Name, job.SourceDir, job.TargetDir, (job.Type));
                }

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

      
    }
}