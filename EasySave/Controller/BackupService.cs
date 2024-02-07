using EasySave.Controller;
using EasySave.Controller.Interfaces;
using EasySave.Model;
using Microsoft.Extensions.Configuration;
using static EasySave.Model.Enum;

namespace EasySave.ViewModel
{
    internal class BackupService : IBackupService
    {
        private static IConfiguration _configuration;
        private IStateLogService _stateLogService;
        private BackupState _backupState;
        private long maxFileSize = 1024 * 1024 * 100; // 100 Mo
        private List<string> allowedFormats = new List<string> { ".txt", ".docx", ".xlsx" };

        public BackupService(IConfiguration configuration)
        {
            _configuration = configuration;
            _stateLogService = new StateLogService(_configuration);
        }

        public void ExecuteBackupJob(BackupJob job)
        {
            if (job == null)
            {
                Console.WriteLine("Le travail de sauvegarde est vide.");
                return;
            }

            Console.WriteLine($"Exécution du travail de sauvegarde : {job.Name}");

            int totalFilesToCopy = new DirectoryInfo(job.SourceDir).EnumerateFiles("*", SearchOption.AllDirectories).Count();
            long totalFilesSize = (int)new DirectoryInfo(job.SourceDir).EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);

            try
            {
                if (!Directory.Exists(job.SourceDir))
                {
                    Console.WriteLine("Le répertoire source n'existe pas.");
                    return;
                }

                if (!Directory.Exists(job.TargetDir))
                {
                    Directory.CreateDirectory(job.TargetDir);
                }

                if (job.Type == JobTypeEnum.full)
                {
                    CopyFullBackup(job, totalFilesToCopy, totalFilesSize);
                }
                else if (job.Type == JobTypeEnum.differential)
                {
                    CopyDifferentialBackup(job, totalFilesToCopy, totalFilesSize);
                }
                _backupState = new BackupState(job.Id, job.Name, DateTime.Now, "END", totalFilesToCopy, totalFilesSize, 0, 0, job.SourceDir, job.TargetDir);
                _stateLogService.UpdateStateLog(_backupState);
                Console.WriteLine($"La sauvegarde a été effectuée avec succès !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur s'est produite lors de la sauvegarde : {ex.Message}");
            }
        }

        private void CopyFullBackup(BackupJob job, int totalFilesToCopy, long totalFilesSize)
        {
            int fileCount = 0; // Nombre de fichiers copiés

            string[] sourceFiles = Directory.GetFiles(job.SourceDir, "*", SearchOption.AllDirectories);

            foreach (string sourceFile in sourceFiles)
            {
                Save(sourceFile, job, fileCount, totalFilesToCopy, totalFilesSize);
                fileCount++;
            }

            Console.WriteLine($"La copie terminée ! Nombre total de fichiers copiés : {fileCount}");
        }

        private void CopyDifferentialBackup(BackupJob job, int totalFilesToCopy, long totalFilesSize)
        {
            int fileCount = 0; // Nombre de fichiers copiés

            string[] sourceFiles = Directory.GetFiles(job.SourceDir, "*", SearchOption.AllDirectories);

            foreach (string sourceFile in sourceFiles)
            {
                FileInfo originalFile = new FileInfo(sourceFile);
                FileInfo destFile = new FileInfo(sourceFile.Replace(job.SourceDir, job.TargetDir));

                if (!destFile.Exists || originalFile.LastWriteTime > destFile.LastWriteTime)
                {
                    Save(sourceFile, job, fileCount, totalFilesToCopy, totalFilesSize);
                    fileCount++;
                }
            }

            Console.WriteLine($"La copie terminée ! Nombre total de fichiers copiés : {fileCount}");
        }

        private void Save(string sourceFile, BackupJob job, int fileCount, int totalFilesToCopy, long totalFilesSize)
        {
            FileInfo fileInfo = new FileInfo(sourceFile);

            if (fileInfo.Length <= maxFileSize)
            {
                if (allowedFormats.Contains(fileInfo.Extension.ToLower()))
                {

                    string targetFilePath = sourceFile.Replace(job.SourceDir, job.TargetDir);
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));

                    long totalSizeTarget = new DirectoryInfo(job.TargetDir).EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
                    long nbFilesSizeLeftToDo = totalFilesSize - totalSizeTarget;
                    int nbFilesLeftToDo = totalFilesToCopy - fileCount;


                    _backupState = new BackupState(job.Id, job.Name, DateTime.Now, "ACTIVE", totalFilesToCopy, totalFilesSize, nbFilesLeftToDo, nbFilesSizeLeftToDo, job.SourceDir, job.TargetDir);
                    _stateLogService.UpdateStateLog(_backupState);

                    File.Copy(sourceFile, targetFilePath, true);
                    Console.WriteLine($"Copie du fichier : {sourceFile}");
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
}