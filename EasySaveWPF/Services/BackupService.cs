using EasySaveWPF.Model;
using EasySaveWPF.Services.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using static EasySaveWPF.Model.Enum;

namespace EasySaveWPF.Services
{


    public class BackupService : IBackupService
    {
        private IStateLogService _stateLogService;
        private BackupState _currentBackupState;
        private int fileCount = 0;
        public long encryptTime = 0;
        private string _filesToEncrypt;

        public BackupService()
        {
            _stateLogService = new StateLogService();
            _filesToEncrypt = Properties.Settings.Default.FilesToEncrypt;
        }
        public event EventHandler<BackupState> CurrentBackupStateChanged;

        public void ExecuteBackupJob(BackupJob job)
        {
            encryptTime = 0;
            if (job == null)
            {
                Console.WriteLine("Le travail de sauvegarde est vide.");
                return;
            }
            Console.WriteLine(String.Format(Resources.Translation.job_execution, job.Name));
            try
            {
                if (!Directory.Exists(job.SourceDir))
                {

                    string messageBoxText = Resources.Translation.source_directory_doesnt_exist;
                    string caption = "Erreur";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Warning;

                    MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

                    Console.WriteLine();
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
                    CopyFullBackup(job, sourceFiles);
                }
                else if (job.Type == JobTypeEnum.differential)
                {
                    CopyDifferentialBackup(job, sourceFiles);
                }
                else
                {
                    Console.WriteLine(Resources.Translation.save_type_error);
                }
                _currentBackupState = new BackupState(job.Id, job.Name, DateTime.Now, "END", 0, 0, 0, 0, "", "");
                _stateLogService.UpdateStateLog(_currentBackupState);
                Console.WriteLine(String.Format(Resources.Translation.copy_success, fileCount));
                Console.WriteLine(Resources.Translation.backup_success);
                fileCount = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format(Resources.Translation.backup_error, ex.Message));
            }
        }
        private void CopyFullBackup(BackupJob job, string[] sourceFiles)
        {
            int totalFilesToCopy = sourceFiles.Length;
            long totalFilesSize = sourceFiles.Select(file => new FileInfo(file).Length).Sum();
            long totalSizeTarget = 0;
            foreach (string sourceFile in sourceFiles)
            {
                FileInfo originalFile = new FileInfo(sourceFile);
                totalSizeTarget += originalFile.Length;
                long nbFilesSizeLeftToDo = totalFilesSize - totalSizeTarget;
                fileCount++;
                Save(sourceFile, job, totalFilesToCopy, totalFilesSize, nbFilesSizeLeftToDo);
            }
        }
        private void CopyDifferentialBackup(BackupJob job, string[] sourceFiles)
        {
            int totalFilesToCopy = 0;
            long totalFilesSize = 0;
            long totalSizeTarget = 0;

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
                    totalSizeTarget += originalFile.Length;
                    long nbFilesSizeLeftToDo = totalFilesSize - totalSizeTarget;
                    Save(sourceFile, job, totalFilesToCopy, totalFilesSize, nbFilesSizeLeftToDo);
                    fileCount++;
                }
            }

        }

        private void Save(string sourceFile, BackupJob job, int totalFilesToCopy, long totalFilesSize, long nbFilesSizeLeftToDo)
        {
            FileInfo fileInfo = new FileInfo(sourceFile);
            string targetFilePath = sourceFile.Replace(job.SourceDir, job.TargetDir);
            int nbFilesLeftToDo = totalFilesToCopy - fileCount;
            _currentBackupState = new BackupState(job.Id, job.Name, DateTime.Now, "ACTIVE", totalFilesToCopy, totalFilesSize, nbFilesLeftToDo, nbFilesSizeLeftToDo, sourceFile, targetFilePath);
            _stateLogService.UpdateStateLog(_currentBackupState);
            OnCurrentBackupStateChanged(_currentBackupState);
            if (_filesToEncrypt.Contains(fileInfo.Extension))
            {

                try
                {

                    Process cryptoSoft = new Process();
                    cryptoSoft.StartInfo.FileName = ".\\CryptoSoft\\CryptoSoft.exe";
                    cryptoSoft.StartInfo.Arguments = $"\"{sourceFile}\" \"{targetFilePath}\"";
                    cryptoSoft.StartInfo.CreateNoWindow = true;
                    cryptoSoft.EnableRaisingEvents = true;
                    cryptoSoft.Exited += ExitEvent;
                    cryptoSoft.Start();
                    cryptoSoft.WaitForExit();

                    if (encryptTime >= 0)
                    {
                        encryptTime += cryptoSoft.ExitCode;
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                File.Copy(sourceFile, targetFilePath, true);
            }
        }
        private void ExitEvent(object sender, EventArgs e)
        {
            Process process = (Process)sender;

            if (process.ExitCode == 0)
            {
                Console.WriteLine("Le processus s'est terminé avec succès.");
            }
            else
            {
                Console.WriteLine("Le processus s'est terminé avec un code de sortie différent de zéro.");
            }
        }

        public long GetEncryptTime()
        {
            return encryptTime;
        }


        protected virtual void OnCurrentBackupStateChanged(BackupState backupState)
        {
            CurrentBackupStateChanged?.Invoke(this, backupState);
        }
    }
}
