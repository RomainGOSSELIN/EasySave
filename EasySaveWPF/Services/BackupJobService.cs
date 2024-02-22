using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySaveWPF.Model;
using EasySaveWPF.Services.Interfaces;
using System;
using EasySaveWPF.Model.LogFactory;
using System.Windows;

namespace EasySaveWPF.Services
{
    public class BackupJobService : IBackupJobService
    {
        private static string _jobsFilePath;
        private static ILogger _logger;
        private static LoggerFactory loggerFactory = new LoggerFactory();
        Notifications.Notifications notifications = new Notifications.Notifications();

        public BackupJobService()
        {
            _jobsFilePath = Properties.Settings.Default.JobsFilepath;
            _logger = loggerFactory.CreateLogger(Model.Enum.LogType.Json);
        }

        public bool CreateJob(BackupJob backupJob)
        {
            List<BackupJob> jobs = _logger.GetLog<BackupJob>(_jobsFilePath) ?? new List<BackupJob>(); ;

            if (backupJob.SourceDir == backupJob.TargetDir)
            {
                notifications.TargetSourceDifferent();
                return false;
            }
            if (!System.IO.Directory.Exists(backupJob.SourceDir))
            {
                notifications.SourceDirNotExist(backupJob.SourceDir);
                return false;
            }

            if (backupJob.Name == "" || backupJob.Name == null)
            {
                notifications.NameEmpty();
                return false;
            }
            if (backupJob.TargetDir == "" || backupJob.TargetDir == null)
            {
                notifications.TargetEmpty();
                return false;
            }
            backupJob.Id = jobs.Count + 1;
            jobs.Add(backupJob);
            _logger.SaveLog(jobs, _jobsFilePath);
            notifications.CreateBackupjob(backupJob.Name, backupJob.Id, backupJob.SourceDir, backupJob.TargetDir, backupJob.Type);
            return true;
        }

        public BackupJob? GetJob(int id)
        {
            List<BackupJob> jobs = _logger.GetLog<BackupJob>(_jobsFilePath);
            BackupJob? backupJob;

            if (jobs == null)
            {
                notifications.JobNotExist(id);
                return null;
            }

            else
            {
                backupJob = jobs.Find(j => j.Id == id);
                if (backupJob == null)
                {
                    notifications.JobNotExist(id);
                    return null;
                }
            }
            return backupJob;
        }
        public List<BackupJob> GetJobs(List<int> ids)
        {
            List<BackupJob> jobs = new List<BackupJob>();
            foreach (int id in ids)
            {
                jobs.Add(GetJob(id));
            }

            return jobs;
        }

        public List<BackupJob> GetAllJobs()
        {
            return _logger.GetLog<BackupJob>(_jobsFilePath);
        }

        public bool DeleteJob(BackupJob job)
        {

            List<BackupJob> jobs = _logger.GetLog<BackupJob>(_jobsFilePath);

            if (job != null)
            {
                jobs.RemoveAll(x => x.Id == job.Id);
                UpdateIds(jobs);
                _logger.SaveLog(jobs, _jobsFilePath);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateJob(BackupJob job)
        {
            List<BackupJob> jobs = GetAllJobs();
           foreach (BackupJob backupJob in jobs)
            {
                if (backupJob.Id == job.Id) 
                {
                    backupJob.State = job.State;
                };
            }
            _logger.SaveLog(jobs, _jobsFilePath);

        }

        private void UpdateIds(List<BackupJob> jobs)
        {
            for (int i = 0; i < jobs.Count; i++)
            {
                jobs[i].Id = i + 1;
            }
        }



    }
}