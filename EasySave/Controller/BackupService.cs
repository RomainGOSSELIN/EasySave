using EasySave.Controller.Interfaces;
using EasySave.Model;
using static EasySave.Model.Enum;

namespace EasySave.Controller
{
    internal class BackupService : IBackupService
    {
        public void ExecuteBackupJob(BackupJob job)
        {
            if (job != null)
            {
                Console.WriteLine($"Exécution du travail de sauvegarde : {job.Name}");

                long maxFileSize = 1024 * 1024 * 100; // 100 Mo
                List<string> allowedFormats = new List<string> { ".txt", ".docx", ".xlsx" };

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

                        int fileCount = 0; // Nombre de fichiers copiés
                                           // Copier les fichiers en fonction du type de sauvegarde
                        if (job.Type == JobTypeEnum.full)
                        {
                            Console.WriteLine("Copie des fichiers...");
                            string[] sourceFiles = Directory.GetFiles(job.SourceDir, "*", SearchOption.AllDirectories);
                            foreach (string sourceFile in sourceFiles)
                            {
                                FileInfo fileInfo = new FileInfo(sourceFile);
                                // Vérifier la taille maximale du fichier
                                if (fileInfo.Length <= maxFileSize)
                                {
                                    // Vérifier le format de sauvegarde du fichier
                                    if (allowedFormats.Contains(fileInfo.Extension.ToLower()))
                                    {
                                        string targetFilePath = sourceFile.Replace(job.SourceDir, job.TargetDir);
                                        Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));

                                        File.Copy(sourceFile, targetFilePath, true);
                                        Console.WriteLine($"Copie du fichier : {sourceFile}");
                                        fileCount++;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Le format du fichier {sourceFile} n'est pas autorisé pour la sauvegarde.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Le fichier {sourceFile} dépasse la taille maximale autorisée.");
                                }
                            }
                            Console.WriteLine("Copie terminée !");
                        }
                        else if (job.Type == JobTypeEnum.differential)
                        {
                            Console.WriteLine("Copie des fichiers différentielle...");
                            // Obtenez la liste des fichiers modifiés ou nouveaux
                            string[] sourceFiles = Directory.GetFiles(job.SourceDir, "*", SearchOption.AllDirectories);
                            foreach (string sourceFile in sourceFiles)
                            {
                                FileInfo originalFile = new FileInfo(sourceFile);
                                FileInfo destFile = new FileInfo(sourceFile.Replace(job.SourceDir, job.TargetDir));

                                if (!destFile.Exists || originalFile.LastWriteTime > destFile.LastWriteTime)
                                {
                                    // Vérifier la taille maximale du fichier
                                    if (originalFile.Length <= maxFileSize)
                                    {
                                        // Vérifier le format de sauvegarde du fichier
                                        if (allowedFormats.Contains(originalFile.Extension.ToLower()))
                                        {
                                            Directory.CreateDirectory(destFile.DirectoryName);
                                            originalFile.CopyTo(destFile.FullName, true);
                                            Console.WriteLine($"Copie du fichier : {sourceFile}");
                                            fileCount++;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Le format du fichier {sourceFile} n'est pas autorisé pour la sauvegarde.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Le fichier {sourceFile} dépasse la taille maximale autorisée.");
                                    }
                                }
                            }
                            Console.WriteLine("Copie terminée !");
                        }

                        Console.WriteLine($"La sauvegarde a été effectuée avec succès ! Nombre total de fichiers copiés : {fileCount}");
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
            else
            {
                Console.WriteLine("Le travail de sauvegarde est vide.");
            }
        }
    }
}
