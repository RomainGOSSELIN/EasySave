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
        string message;
        string caption;
        MessageBoxButton button;
        MessageBoxImage icon;

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
                message = Resources.Translation.target_source_must_be_different;
                caption = Resources.Translation.error;
                button = MessageBoxButton.OK;
                icon = MessageBoxImage.Error;
                MessageBox.Show(message, caption, button, icon);

                return false;
            }
            backupJob.Id = jobs.Count + 1;
            if (!System.IO.Directory.Exists(backupJob.SourceDir))
            {

                message = Resources.Translation.source_directory_doesnt_exist;
                caption = Resources.Translation.error;
                button = MessageBoxButton.OK;
                icon = MessageBoxImage.Error;
                MessageBox.Show(message, caption, button, icon);
                return false;
            }

            if (backupJob.Name == "" || backupJob.Name == null)
            {
                message = Resources.Translation.name_empty;
                caption = Resources.Translation.error;
                button = MessageBoxButton.OK;
                icon = MessageBoxImage.Error;
                MessageBox.Show(message, caption, button, icon);
                return false;
            }
            if (backupJob.TargetDir == "" || backupJob.TargetDir == null)
            {
                message = Resources.Translation.target_empty;
                caption = Resources.Translation.error;
                button = MessageBoxButton.OK;
                icon = MessageBoxImage.Error;
                MessageBox.Show(message, caption, button, icon);
                return false;
            }

            jobs.Add(backupJob);
            _logger.SaveLog(jobs, _jobsFilePath);
            message = String.Format(Resources.Translation.create_job_success, backupJob.Name, backupJob.Id, backupJob.SourceDir, backupJob.TargetDir, backupJob.Type);
            caption = Resources.Translation.creation;
            button = MessageBoxButton.OK;
            icon = MessageBoxImage.Information;
            MessageBox.Show(message, caption, button, icon);


            return true;


        }
        public BackupJob? GetJob(int id)
        {
            List<BackupJob> jobs = _logger.GetLog<BackupJob>(_jobsFilePath);
            BackupJob? backupJob;

            if (jobs == null)
            {
                message = String.Format(Resources.Translation.job_doesnt_exist, id);
                caption = Resources.Translation.error;
                button = MessageBoxButton.OK;
                icon = MessageBoxImage.Error;
                MessageBox.Show(message, caption, button, icon);
                return null;
            }

            else
            {
                backupJob = jobs.Find(j => j.Id == id);
                if (backupJob == null)
                {
                    message = String.Format(Resources.Translation.job_doesnt_exist, id);
                    caption = Resources.Translation.error;
                    button = MessageBoxButton.OK;
                    icon = MessageBoxImage.Error;
                    MessageBox.Show(message, caption, button, icon);
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
