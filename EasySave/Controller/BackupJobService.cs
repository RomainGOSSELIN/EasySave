using EasySave.Controller.Interfaces;
using EasySave.Model;
using Microsoft.Extensions.Configuration;

namespace EasySave.ViewModel
{
    public class BackupJobService : IBackupJobService
    {
        private readonly IConfiguration _configuration;
        private static string _jobsFilePath;
        public BackupJobService(IConfiguration configuration)
        {
            _configuration = configuration;
            _jobsFilePath = _configuration["AppConfig:JobsFilePath"];
        }

        private JsonService _jsonService = new JsonService();
        public bool CreateJob(BackupJob backupJob)
        {

            List<BackupJob> jobs = _jsonService.GetLog<BackupJob>(_jobsFilePath) ?? new List<BackupJob>(); ;


            if (jobs.Count < 5)
            {
                backupJob.Id = jobs.Count + 1;
                jobs.Add(backupJob);
                _jsonService.SaveLog(jobs, _jobsFilePath);
                Console.WriteLine($"Le travail {backupJob.Name} a été créé à l'emplacement {backupJob.Id} depuis {backupJob.SourceDir} à {backupJob.TargetDir} avec un type {backupJob.Type}");
                return true;
            }
            else
            {
                Console.WriteLine("Vous ne pouvez pas créer plus de 5 travaux.");
                return false;
            }


            //// Chemin du fichier JSON
            //string cheminFichierJson = ".\\Jobs.json";

            //var sauvegardesExistantes = JsonService.GetLog<BackupJob>(cheminFichierJson);

            //// Vérification de l'ID et du nombre total d'éléments
            //if (backupJob.Id <= 0)
            //{
            //    // Recherche de l'ID disponible le plus bas
            //    int nouvelId = 1;
            //    while (sauvegardesExistantes.Any(s => s.Id == nouvelId))
            //    {
            //        nouvelId++;
            //    }
            //    backupJob.Id = nouvelId;
            //}

            //if (backupJob.Id > 5)
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;

            //    Console.WriteLine("Vous ne pouvez pas créer plus de 5 travaux.");
            //    Console.ResetColor();
            //    return;
            //}

            //var existingSauvegarde = sauvegardesExistantes.Find(s => s.Id == backupJob.Id);

            //if (existingSauvegarde != null)
            //{
            //    // Si l'ID existe déjà, mettre à jour les informations
            //    existingSauvegarde.Name = backupJob.Name;
            //    existingSauvegarde.SourceDir = backupJob.SourceDir;
            //    existingSauvegarde.TargetDir = backupJob.TargetDir;
            //    existingSauvegarde.Type = backupJob.Type;
            //}
            //else
            //{
            //    // Sinon, ajouter la nouvelle sauvegarde à la liste existante
            //    sauvegardesExistantes.Add(backupJob);
            //}

            //JsonService.SaveLog(sauvegardesExistantes, cheminFichierJson);


            //Console.WriteLine($"Le travail {backupJob.Name} a été créé à l'emplacement {backupJob.Id} depuis {backupJob.SourceDir} à {backupJob.TargetDir} avec un type {backupJob.Type}");
        }
        public BackupJob? GetJob(int id)
        {
             List<BackupJob> jobs = _jsonService.GetLog<BackupJob>(_jobsFilePath);
             BackupJob? backupJob;

            if (jobs == null)
            {
                Console.WriteLine($"Backup job {id} n'existe pas");
                return null;
            }

            else
            {
                backupJob = jobs.Find(j => j.Id == id);
                if (backupJob == null)
                {
                    Console.WriteLine($"Backup job {id} n'existe pas");
                }
            }
           

            return backupJob;


        }
        public List<BackupJob> GetJobs(List<int> ids)
        {
            List<BackupJob> jobs = new List<BackupJob>();
            foreach (int id in ids)
            {
                jobs.Add(GetJob(id));
            }

            return jobs;
        }

        public List<BackupJob> GetAllJobs()
        {
            return _jsonService.GetLog<BackupJob>(_jobsFilePath);
        }

        public bool DeleteJob(int idToDelete)
        {

            List<BackupJob> jobs = _jsonService.GetLog<BackupJob>(_jobsFilePath);
            BackupJob? jobToDelete = jobs.Find(j => j.Id == idToDelete);

            if (jobToDelete != null)
            {
                jobs.Remove(jobToDelete);
                UpdateIds(jobs);
                _jsonService.SaveLog(jobs, _jobsFilePath);
                Console.WriteLine($"Le travail numéro {idToDelete} a été supprimé avec succès.");
                return true;
            }
            else
            {
                Console.WriteLine($"Aucune information trouvée avec l'ID {idToDelete}.");
                return false;
            }
        }

        private void UpdateIds(List<BackupJob> jobs)
        {
            for (int i = 0; i < jobs.Count; i++)
            {
                jobs[i].Id = i + 1;
            }
        }



    }
}
