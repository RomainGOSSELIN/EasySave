using EasySave.Model;
using Microsoft.Extensions.Configuration;

namespace EasySave.ViewModel
{
    internal class BackupJobService
    {
        private readonly IConfiguration _configuration;

        private JsonService JsonService = new JsonService();
        public void CreateJob(BackupJob backupJob)
        {
            string cheminFichierJson = ".\\Jobs.json";

            List<BackupJob> jobs = JsonService.GetLog<BackupJob>(cheminFichierJson);
           

            if (jobs.Count < 5)
            {
                backupJob.Id = jobs.Count + 1;
                jobs.Add(backupJob);
                JsonService.SaveLog(jobs, cheminFichierJson);
                Console.WriteLine($"Le travail {backupJob.Name} a été créé à l'emplacement {backupJob.Id} depuis {backupJob.SourceDir} à {backupJob.TargetDir} avec un type {backupJob.Type}");
            }
            else
            {
                Console.WriteLine("Vous ne pouvez pas créer plus de 5 travaux.");
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

        public BackupJobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public BackupJob? GetJob(int id)
        {
            string cheminFichierJson = _configuration["AppConfig:JobsFilePath"]; ;

            BackupJob? backupJob = JsonService.GetLog<BackupJob>(cheminFichierJson).Find(s => s.Id == id);

            if (backupJob == null)
            {
                Console.WriteLine($"Backup job {id} n'existe pas");
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
            string filePath = ".\\Jobs.json";
            //configuration["AppConfig:Language"];
            return JsonService.GetLog<BackupJob>(filePath);
        }

        public void DeleteJob(int idToDelete)
        {
            string filePath = ".\\Jobs.json";

            List<BackupJob> jobs = JsonService.GetLog<BackupJob>(filePath);
            BackupJob? jobToDelete = jobs.Find(j => j.Id == idToDelete);

            if (jobToDelete != null)
            {
                jobs.Remove(jobToDelete);
                UpdateIds(jobs);
                JsonService.SaveLog(jobs,filePath);
                Console.WriteLine($"Le travail numéro {idToDelete} a été supprimé avec succès.");
            }
            else
            {
                Console.WriteLine($"Aucune information trouvée avec l'ID {idToDelete}.");
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
