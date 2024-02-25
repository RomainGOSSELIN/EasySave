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
        private string _filesToEncrypt;
        Notifications.Notifications notifications = new Notifications.Notifications();
        private static readonly object _statusLock = new object();
        private static readonly object _lock = new object();
        private static readonly object _maxSizeLock = new object();
        private static Barrier priorityFilesBarrier = new Barrier(0);
        private static Barrier preAnalyzeBarrier = new Barrier(0);

        private static string _priorityExtensions = Properties.Settings.Default.PriorityFiles;

        public BackupService()
        {
            _backupJobService = new BackupJobService();
            _filesToEncrypt = Properties.Settings.Default.FilesToEncrypt;
        }
        public event EventHandler<BackupJob> CurrentBackupStateChanged;

        public void ExecuteBackupJob(BackupJob job)
        {
            preAnalyzeBarrier.AddParticipant();
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
                job.State.EncryptionTime = 0;


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
                    job.State = new BackupState(DateTime.Now, StateEnum.END, 0, 0, 0, 0, "", "", job.State.EncryptionTime);
                    _backupJobService.UpdateJob(job);
                    OnCurrentBackupStateChanged(job);

                }
            }
            catch (Exception ex)
            {
                notifications.BackupError(ex.Message);
            }

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

                else if (new FileInfo(sourceFile).Length > 500)
                {
                    lock (_maxSizeLock)
                    {
                        Save(sourceFile, job);
                    }

                }
                else
                {
                    Save(sourceFile, job);
                }

            }

            priorityFilesBarrier.SignalAndWait();

            foreach (string sourceFile in nonPriorityFiles)
            {

                if (job.CancellationTokenSource.IsCancellationRequested)
                {
                    priorityFilesBarrier.RemoveParticipant();

                    return;
                }
                else if (new FileInfo(sourceFile).Length > 500)
                {
                    lock (_maxSizeLock)
                    {
                        Save(sourceFile, job);
                    }

                }
                else
                {
                    Save(sourceFile, job);
                }
            }

            priorityFilesBarrier.RemoveParticipant();

        }
        private void CopyDifferentialBackup(BackupJob job, List<string> sourceFiles, List<string> nonPriorityFiles)
        {

            priorityFilesBarrier.AddParticipant();

            preAnalyzeBarrier.SignalAndWait();

            preAnalyzeBarrier.RemoveParticipant();


            foreach (string sourceFile in sourceFiles)
            {
                FileInfo originalFile = new FileInfo(sourceFile);
                FileInfo destFile = new FileInfo(sourceFile.Replace(job.SourceDir, job.TargetDir));

                if (job.CancellationTokenSource.IsCancellationRequested)
                {
                    priorityFilesBarrier.RemoveParticipant();
                    return;
                }
                else if (!destFile.Exists || originalFile.LastWriteTime > destFile.LastWriteTime)
                {
                    if (originalFile.Length > 500)
                    {
                        lock (_maxSizeLock)
                        {
                            Save(sourceFile, job);
                        }

                    }
                    else
                    {
                        Save(sourceFile, job);
                    }
                }


            }
            priorityFilesBarrier.SignalAndWait();

            foreach (string sourceFile in nonPriorityFiles)
            {
                FileInfo originalFile = new FileInfo(sourceFile);
                FileInfo destFile = new FileInfo(sourceFile.Replace(job.SourceDir, job.TargetDir));
                if (job.CancellationTokenSource.IsCancellationRequested)
                {
                    priorityFilesBarrier.RemoveParticipant();
                    return;
                }
                else if (!destFile.Exists || originalFile.LastWriteTime > destFile.LastWriteTime)
                {
                    if (originalFile.Length > 500)
                    {
                        lock (_maxSizeLock)
                        {
                            Save(sourceFile, job);
                        }

                    }
                    else
                    {
                        Save(sourceFile, job);
                    }
                }
            }
            priorityFilesBarrier.RemoveParticipant();


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

                    if (job.State.EncryptionTime >= 0)
                    {
                        job.State.EncryptionTime += cryptoSoft.ExitCode;
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

            job.State = new BackupState(DateTime.Now, StateEnum.ACTIVE, job.State.TotalFilesToCopy, job.State.TotalFilesSize, job.State.NbFilesLeftToDo - 1, job.State.NbFilesSizeLeftToDo - fileInfo.Length, sourceFile, targetFilePath);


            lock (_statusLock)
            {
                _backupJobService.UpdateJob(job);
                OnCurrentBackupStateChanged(job);

            }
        }



        protected virtual void OnCurrentBackupStateChanged(BackupJob job)
        {
            CurrentBackupStateChanged?.Invoke(this, job);
        }
    }
}