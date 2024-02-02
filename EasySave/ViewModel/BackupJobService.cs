﻿using EasySave.Model;
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
                    // Si aucun ID n'est indiqué, incrémenter automatiquement l'ID
                    backupJob.Id = sauvegardesExistantes.Count + 1;
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
    }
}
    