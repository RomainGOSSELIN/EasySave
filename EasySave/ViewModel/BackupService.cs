using EasySave.Model;
using static EasySave.Model.Enum;

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
                        // Vérifier si le répertoire cible existe
                        if (!Directory.Exists(job.TargetDir))
                        {
                            Directory.CreateDirectory(job.TargetDir);
                        }

                        // Copier les fichiers en fonction du type de sauvegarde
                        string[] sourceFiles = Directory.GetFiles(job.SourceDir, "*", SearchOption.AllDirectories);
                        if (job.Type == JobTypeEnum.full)
                        {
                            Console.WriteLine("Copie des fichiers...");
                            foreach (string sourceFile in sourceFiles)
                            {
                                string targetFilePath = sourceFile.Replace(job.SourceDir, job.TargetDir);
                                Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));
                                File.Copy(sourceFile, targetFilePath, true);
                                Console.WriteLine($"Copie du fichier : {sourceFile}");
                            }
                            Console.WriteLine("Copie terminée !");
                        }
                        else if (job.Type == JobTypeEnum.differential)
                        {
                            Console.WriteLine("Copie des fichiers différentielle...");
                            // Obtenez la liste des fichiers modifiés ou nouveaux
                            foreach (string sourceFile in sourceFiles)
                            {
                                FileInfo originalFile = new FileInfo(sourceFile);
                                FileInfo destFile = new FileInfo(sourceFile.Replace(job.SourceDir, job.TargetDir));

                                if (!destFile.Exists || originalFile.LastWriteTime > destFile.LastWriteTime)
                                {
                                    Directory.CreateDirectory(destFile.DirectoryName);
                                    originalFile.CopyTo(destFile.FullName, true);
                                    Console.WriteLine($"Copie du fichier : {sourceFile}");
                                }
                            }
                            Console.WriteLine("Copie terminée !");
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
