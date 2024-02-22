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
        private BackupJobService _backupJobService;
        private BackupState _currentBackupState;
        private int fileCount = 0;
        public long encryptTime = 0;
        private string _filesToEncrypt;
        Notifications.Notifications notifications = new Notifications.Notifications();

        public BackupService()
        {
            _stateLogService = new StateLogService();
            _backupJobService = new BackupJobService();
            _filesToEncrypt = Properties.Settings.Default.FilesToEncrypt;
            _statusSemaphore = new Semaphore(1, 1);
        }
        public event EventHandler<BackupState> CurrentBackupStateChanged;

        public void ExecuteBackupJob(BackupJob job)
        {
            encryptTime = 0;
            if (job == null)
            {
                notifications.JobNotExist(job.Id);
                return;
            }
            try
            {
                if (!Directory.Exists(job.SourceDir))
                {
                    notifications.SourceDirNotExist(job.SourceDir);
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
                _currentBackupState = new BackupState(job.Id, job.Name, DateTime.Now, "END", 0, 0, 0, 0, "", "");
                _stateLogService.UpdateStateLog(_currentBackupState);
                job.State = _currentBackupState;
                _backupJobService.UpdateJob(job);
                OnCurrentBackupStateChanged(_currentBackupState);

                Console.WriteLine(String.Format(Resources.Translation.copy_success, fileCount));
                Console.WriteLine(Resources.Translation.backup_success);
                fileCount = 0;
            }
            catch (Exception ex)
            {
                notifications.BackupError(ex.Message);
            }
        }
        private void CopyFullBackup(BackupJob job, string[] sourceFiles)
        {
            int totalFilesToCopy = sourceFiles.Length;
            long totalFilesSize = sourceFiles.Select(file => new FileInfo(file).Length).Sum();
            long totalSizeTarget = 0;
            job.State.NbFilesLeftToDo = totalFilesToCopy;

            foreach (string sourceFile in sourceFiles)
            {
                FileInfo originalFile = new FileInfo(sourceFile);
                totalSizeTarget += originalFile.Length;
                fileCount++;
                long nbFilesSizeLeftToDo = totalFilesSize - totalSizeTarget;
                job.State.TotalFilesToCopy = totalFilesToCopy;
                job.State.TotalFilesSize = totalFilesSize;

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

            if (_filesToEncrypt.Contains(fileInfo.Extension))
            {
                try
                {
                    Process cryptoSoft = new Process();
                    cryptoSoft.StartInfo.FileName = ".\\CryptoSoft\\CryptoSoft.exe";
                    cryptoSoft.StartInfo.Arguments = $"\"{sourceFile}\" \"{targetFilePath}\"";
                    cryptoSoft.StartInfo.CreateNoWindow = true;
                    cryptoSoft.EnableRaisingEvents = true;
                    cryptoSoft.StartInfo.WorkingDirectory = ".\\CryptoSoft";
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
            int nbFilesLeftToDo = job.State.NbFilesLeftToDo - 1;

            _currentBackupState = new BackupState(job.Id, job.Name, DateTime.Now, "ACTIVE", job.State.TotalFilesToCopy, job.State.TotalFilesSize, nbFilesLeftToDo, nbFilesSizeLeftToDo, sourceFile, targetFilePath);
            _statusSemaphore.WaitOne();
            _stateLogService.UpdateStateLog(_currentBackupState);
            _statusSemaphore.Release();

            job.State = _currentBackupState;
            _backupJobService.UpdateJob(job);
            OnCurrentBackupStateChanged(_currentBackupState);
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