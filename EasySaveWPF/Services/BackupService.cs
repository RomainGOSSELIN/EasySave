using EasySaveWPF.Model;
using EasySaveWPF.Services.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using static EasySaveWPF.Model.Enum;

namespace EasySaveWPF.Services
{


    public class BackupService : IBackupService
    {
        private BackupJobService _backupJobService;
        private BackupState _currentBackupState;
        public long encryptTime = 0;
        private string _filesToEncrypt;
        Notifications.Notifications notifications = new Notifications.Notifications();
        private static readonly object _statusLock = new object();
        private static Barrier priorityFilesBarrier = new Barrier(0);
        private static Barrier preAnalyzeBarrier = new Barrier(0);

        private static List<string> _priorityExtensions = [".JPG", ".DNG"];

        public BackupService()
        {
            _backupJobService = new BackupJobService();
            _filesToEncrypt = Properties.Settings.Default.FilesToEncrypt;
        }
        public event EventHandler<BackupJob> CurrentBackupStateChanged;

        public void ExecuteBackupJob(BackupJob job)
        {
            preAnalyzeBarrier.AddParticipant();
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

                List<string> priorityFiles = new List<string> { };
                List<string> nonPriorityFiles = new List<string> { };

                foreach (string file in sourceFiles)
                {
                    FileInfo originalFile = new FileInfo(file);
                    if (_priorityExtensions.Contains(originalFile.Extension))
                    {
                        priorityFiles.Add(file);
                    }
                    else
                    {
                        nonPriorityFiles.Add(file);
                    }
                }

                job.State.TotalFilesToCopy = sourceFiles.Length;
                job.State.TotalFilesSize = sourceFiles.Select(file => new FileInfo(file).Length).Sum();
                job.State.State = StateEnum.ACTIVE;
                job.State.Timestamp = DateTime.Now;
                job.State.NbFilesLeftToDo = job.State.TotalFilesToCopy;
                job.State.NbFilesSizeLeftToDo = job.State.TotalFilesSize;


                lock (_statusLock)
                {
                 
                    _backupJobService.UpdateJob(job);
                    OnCurrentBackupStateChanged(job);

                }


                if (job.Type == JobTypeEnum.full)
                {
                    CopyFullBackup(job, priorityFiles, nonPriorityFiles);
                }
                else if (job.Type == JobTypeEnum.differential)
                {
                    CopyDifferentialBackup(job, priorityFiles, nonPriorityFiles);
                }

                lock (_statusLock)
                {
                    _currentBackupState = new BackupState(job.Id, job.Name, DateTime.Now, StateEnum.END, 0, 0, 0, 0, "", "");
                    job.State = _currentBackupState;
                    _backupJobService.UpdateJob(job);
                    OnCurrentBackupStateChanged(job);

                }
            }
            catch (Exception ex)
            {
                notifications.BackupError(ex.Message);
            }
            //preAnalyzeBarrier.RemoveParticipant();

        }
        private void CopyFullBackup(BackupJob job, List<string> sourceFiles, List<string> nonPriorityFiles)
        {

            priorityFilesBarrier.AddParticipant();

            preAnalyzeBarrier.SignalAndWait();

            preAnalyzeBarrier.RemoveParticipant();




            foreach (string sourceFile in sourceFiles)
            {
                if (job.CancellationTokenSource.IsCancellationRequested)
                {
                    priorityFilesBarrier.RemoveParticipant();
                    return;
                }

                Save(sourceFile, job);

            }

            priorityFilesBarrier.SignalAndWait();

            foreach (string sourceFile in nonPriorityFiles)
            {

                if (job.CancellationTokenSource.IsCancellationRequested)
                {
                    priorityFilesBarrier.RemoveParticipant();

                    return;
                }
                Save(sourceFile, job);
            }

            priorityFilesBarrier.RemoveParticipant();

        }
        private void CopyDifferentialBackup(BackupJob job, List<string> sourceFiles, List<string> nonPriorityFiles)
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
                    Save(sourceFile, job);
                }
            }

        }

        private void Save(string sourceFile, BackupJob job)
        {

            FileInfo fileInfo = new FileInfo(sourceFile);
            string targetFilePath = sourceFile.Replace(job.SourceDir, job.TargetDir);

            job.ResetEvent.WaitOne();

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

            _currentBackupState = new BackupState(job.Id, job.Name, DateTime.Now, StateEnum.ACTIVE, job.State.TotalFilesToCopy, job.State.TotalFilesSize, job.State.NbFilesLeftToDo, job.State.NbFilesSizeLeftToDo, sourceFile, targetFilePath);


            job.State.TotalFilesToCopy = _currentBackupState.TotalFilesToCopy;
            job.State.TotalFilesSize = _currentBackupState.TotalFilesSize;
            job.State.NbFilesLeftToDo = _currentBackupState.NbFilesLeftToDo - 1;
            job.State.NbFilesSizeLeftToDo = _currentBackupState.NbFilesSizeLeftToDo - fileInfo.Length;
            job.State.SourceFilePath = _currentBackupState.SourceFilePath;
            job.State.TargetFilePath = _currentBackupState.TargetFilePath;

            lock (_statusLock)
            {
                _backupJobService.UpdateJob(job);
                OnCurrentBackupStateChanged(job);

            }
        }

        public long GetEncryptTime()
        {
            return encryptTime;
        }


        protected virtual void OnCurrentBackupStateChanged(BackupJob job)
        {
            CurrentBackupStateChanged?.Invoke(this, job);
        }
    }
}