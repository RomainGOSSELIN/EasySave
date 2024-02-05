using EasySave.Model;

namespace EasySave.ViewModel
{
    internal class BackupService
    {
        public void ExecuteBackupJob(BackupJob job)
        {
            if (job != null)
            {

                Console.WriteLine($"Exécution du travail de sauvegarde : {job.Name}");

                try
                {
                    // Vérifier si le répertoire source existe
                    if (Directory.Exists(job.SourceDir))
                    {
                        // Créer le répertoire cible s'il n'existe pas encore
                        if (!Directory.Exists(job.TargetDir))
                        {
                            Directory.CreateDirectory(job.TargetDir);
                        }

                        // Obtenir la liste des fichiers dans le répertoire source
                        string[] files = Directory.GetFiles(job.SourceDir);

                        // Copier chaque fichier dans le répertoire cible
                        foreach (string file in files)
                        {
                            string fileName = Path.GetFileName(file);
                            string destFile = Path.Combine(job.TargetDir, fileName);
                            File.Copy(file, destFile, true);
                        }

                        Console.WriteLine("La sauvegarde a été effectuée avec succès !");
                    }
                    else
                    {
                        Console.WriteLine("Le répertoire source n'existe pas.");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Une erreur s'est produite lors de la sauvegarde : {ex.Message}");
                }
            }
        }
    }
}
