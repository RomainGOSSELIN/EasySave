using EasySave.Controller;
using EasySave.Controller.Interfaces;
using EasySave.Model;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using static EasySave.Model.Enum;

namespace EasySave.Controller
{
    internal class BackupService : IBackupService
    {
        private static IConfiguration _configuration;
        private IStateLogService _stateLogService;
        private BackupState _backupState;
        //private long maxFileSize = 1024 * 1024 * 100; // 100 Mo
        //private List<string> allowedFormats = new List<string> { ".txt", ".docx", ".xlsx" };
        private int fileCount = 0;
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
            Console.WriteLine(String.Format(Resources.Translation.job_execution,job.Name));
            try
            {
                if (!Directory.Exists(job.SourceDir))
                {
                    Console.WriteLine(Resources.Translation.source_directory_doesnt_exist);
                    return;
                }

                if (!Directory.Exists(job.TargetDir))
                {
                    Directory.CreateDirectory(job.TargetDir);
                }
                string[] sourceFiles = Directory.GetFiles(job.SourceDir, "*", SearchOption.AllDirectories);
                string[] targetFiles = Directory.GetFiles(job.TargetDir, "*", SearchOption.AllDirectories);
                if (job.Type == JobTypeEnum.full)
                {
                    CopyFullBackup(job, sourceFiles, targetFiles);
                }
                else if (job.Type == JobTypeEnum.differential)
                {
                    CopyDifferentialBackup(job, sourceFiles, targetFiles);
                }
                else
                {
                    Console.WriteLine(Resources.Translation.save_type_error);
                }
                _backupState = new BackupState(job.Id, job.Name, DateTime.Now, "END", 0, 0, 0, 0, "", "");
                _stateLogService.UpdateStateLog(_backupState);
                Console.WriteLine(String.Format(Resources.Translation.copy_success, fileCount));
                Console.WriteLine(Resources.Translation.backup_success);
                fileCount = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format(Resources.Translation.backup_error ,ex.Message));
            }
        }
        private void CopyFullBackup(BackupJob job, string[] sourceFiles, string[] targetFiles)
        {
            int totalFilesToCopy = sourceFiles.Length;
            long totalFilesSize = sourceFiles.Select(file => new FileInfo(file).Length).Sum();
            foreach (string sourceFile in sourceFiles)
            {
                Save(sourceFile, job, totalFilesToCopy, totalFilesSize, targetFiles);
                fileCount++;
            }
        }
        private void CopyDifferentialBackup(BackupJob job, string[] sourceFiles, string[] targetFiles)
        {
            int totalFilesToCopy = 0;
            long totalFilesSize = 0;
            foreach (string sourceFile in sourceFiles)
            {
                FileInfo originalFile = new FileInfo(sourceFile);
                FileInfo destFile = new FileInfo(sourceFile.Replace(job.SourceDir, job.TargetDir));

                if (!destFile.Exists || originalFile.LastWriteTime > destFile.LastWriteTime)
                {
                    totalFilesToCopy++;
                    totalFilesSize += originalFile.Length;
                }   
            }
            foreach (string sourceFile in sourceFiles)
            {
                FileInfo originalFile = new FileInfo(sourceFile);
                FileInfo destFile = new FileInfo(sourceFile.Replace(job.SourceDir, job.TargetDir));

                if (!destFile.Exists || originalFile.LastWriteTime > destFile.LastWriteTime)
                {
                    Save(sourceFile, job, totalFilesToCopy, totalFilesSize, targetFiles);
                    fileCount++;
                }
            }
        }
        private void Save(string sourceFile, BackupJob job, int totalFilesToCopy, long totalFilesSize, string[] targetFiles)
        {
            FileInfo fileInfo = new FileInfo(sourceFile);
            //if (fileInfo.Length <= maxFileSize)
            //{
            //    if (allowedFormats.Contains(fileInfo.Extension.ToLower()))
            //    {
                    string targetFilePath = sourceFile.Replace(job.SourceDir, job.TargetDir);
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));
                    long totalSizeTarget = targetFiles.Select(file => new FileInfo(file).Length).Sum();
                    long nbFilesSizeLeftToDo = totalFilesSize - totalSizeTarget;
                    int nbFilesLeftToDo = totalFilesToCopy - fileCount;
                    _backupState = new BackupState(job.Id, job.Name, DateTime.Now, "ACTIVE", totalFilesToCopy, totalFilesSize, nbFilesLeftToDo, nbFilesSizeLeftToDo, sourceFile, targetFilePath);
                    _stateLogService.UpdateStateLog(_backupState);
                    File.Copy(sourceFile, targetFilePath, true);
                    Console.WriteLine(String.Format(Resources.Translation.copy_file, sourceFile));
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
            //    }
            //    else
            //    {
            //        Console.WriteLine(String.Format(Resources.Translation.incorrect_format, sourceFile));
            //    }
            //}
            //else
            //{
            //    Console.WriteLine(String.Format(Resources.Translation.maxsize_error, sourceFile));
            //}
        }
    }
}