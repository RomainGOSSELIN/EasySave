using EasySave.Model;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace EasySave.ViewModel
{
    internal class BackupJobService
    {
        
            public void CreateJob(BackupJob backupJob)
            {

                // Chemin du fichier JSON
                string cheminFichierJson = ".\\Jobs.json";

                // Lecture du contenu existant du fichier
                string contenuExistant = File.Exists(cheminFichierJson) ? File.ReadAllText(cheminFichierJson) : "";

                // Désérialisation du JSON existant en une liste d'objets
                var sauvegardesExistantes = JsonConvert.DeserializeObject<List<BackupJob>>(contenuExistant) ?? new List<BackupJob>();

                // Vérification de l'ID et du nombre total d'éléments
                if (backupJob.Id <= 0)
            {
                // Recherche de l'ID disponible le plus bas
                int nouvelId = 1;
                while (sauvegardesExistantes.Any(s => s.Id == nouvelId))
                {
                    nouvelId++;
                }
                backupJob.Id = nouvelId;
            }

            if (backupJob.Id > 5)
                {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("Vous ne pouvez pas créer plus de 5 travaux.");
                Console.ResetColor();
                    return;
                }

                var existingSauvegarde = sauvegardesExistantes.Find(s => s.Id == backupJob.Id);

                if (existingSauvegarde != null)
                {
                    // Si l'ID existe déjà, mettre à jour les informations
                    existingSauvegarde.Name = backupJob.Name;
                    existingSauvegarde.SourceDir = backupJob.SourceDir;
                    existingSauvegarde.TargetDir = backupJob.TargetDir;
                    existingSauvegarde.Type = backupJob.Type;
                }
                else
                {
                    // Sinon, ajouter la nouvelle sauvegarde à la liste existante
                    sauvegardesExistantes.Add(backupJob);
                }

                // Conversion de la liste en format JSON
                string json = JsonConvert.SerializeObject(sauvegardesExistantes, Formatting.Indented);

                // Écriture du JSON dans le fichier
                File.WriteAllText(cheminFichierJson, json);

                Console.WriteLine($"Le travail {backupJob.Name} a été créé à l'emplacement {backupJob.Id} depuis {backupJob.SourceDir} à {backupJob.TargetDir} avec un type {backupJob.Type}");
            }



        public BackupJob GetJob(int id)
        {
            string cheminFichierJson = ".\\Jobs.json";
            string contenuExistant = File.Exists(cheminFichierJson) ? File.ReadAllText(cheminFichierJson) : "";
            var sauvegardesExistantes = JsonConvert.DeserializeObject<List<BackupJob>>(contenuExistant) ?? new List<BackupJob>();
            return sauvegardesExistantes.Find(s => s.Id == id);
        }   



        public void DeleteJob (int idToDelete)
        {
            // Spécifiez le chemin du fichier JSON
            string filePath = ".\\Jobs.json";

            // Lire le contenu du fichier JSON
            string jsonString = File.ReadAllText(filePath);

            // Convertir la chaîne JSON en tableau d'objets JObject
            JArray jsonArray = JArray.Parse(jsonString);

            // Spécifiez l'ID que vous souhaitez supprimer
            int idToDelete = 3;

            // Trouver l'objet avec l'ID spécifié et le supprimer du tableau
            JObject itemToRemove = jsonArray
                .FirstOrDefault(item => item["Id"] != null && item["Id"].Value<int>() == idToDelete) as JObject;

            if (itemToRemove != null)
            {
                jsonArray.Remove(itemToRemove);

                // Convertir le tableau modifié en chaîne JSON
                string modifiedJsonString = jsonArray.ToString(Formatting.Indented);

                // Réécrire le fichier avec les données mises à jour
                File.WriteAllText(filePath, modifiedJsonString);

                Console.WriteLine($"Le travail liées à l'ID {idToDelete} ont été supprimées avec succès.");
            }
            else
            {
                Console.WriteLine($"Aucune information trouvée avec l'ID {idToDelete}.");
            }
        }
    }
}
    