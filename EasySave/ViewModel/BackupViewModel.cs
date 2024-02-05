using ConsoleTables;
using EasySave.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasySave.Model.Enum;

namespace EasySave.ViewModel
{
    internal class BackupViewModel
    {
        private static BackupJobService BackupJobService = new BackupJobService();
        private static BackupService BackupService = new BackupService();

        internal void ExecuteJob(string id)
        {
            var separators = new Char [] { ':', ';' };

            if (separators.Any(id.Contains)) {
                List<BackupJob> backupJobs = BackupJobService.GetJobs(id.Split(separators).Select(int.Parse).ToList());

                foreach(var backupJob in backupJobs)
                {
                    BackupService.ExecuteBackupJob(backupJob);
                }
            }
            else {
                BackupService.ExecuteBackupJob(BackupJobService.GetJob(int.Parse(id)));

            }; 
            
        }

        internal void CreateJob(string jobName, string source, string dest , JobTypeEnum type)
        {
            BackupJob backupJob = new BackupJob(jobName, source, dest, type);
            BackupJobService.CreateJob(backupJob);
        }
        internal void DeleteJob(string idToDelete)
        {
            int intIdToDelete = Int32.Parse(idToDelete);
            BackupJobService.DeleteJob(intIdToDelete);
        }

        internal ConsoleTable ShowJob(string id)
        {
            BackupJob job = BackupJobService.GetJob(int.Parse(id));
            var table = new ConsoleTable("Numero", "Nom du Travail", "Source", "Destination", "Type");

            if (job != null)
            {
                table.AddRow(job.Id, job.Name, job.SourceDir, job.TargetDir, (job.Type));

            }
            return table;

        }
    }
}
