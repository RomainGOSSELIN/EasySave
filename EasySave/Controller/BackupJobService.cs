using EasySave.Controller.Interfaces;
using EasySave.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Xml.Linq;

namespace EasySave.Controller
{
    public class BackupJobService : IBackupJobService
    {
        private readonly IConfiguration _configuration;
        private static string _jobsFilePath;
        public BackupJobService(IConfiguration configuration)
        {
            _configuration = configuration;
            _jobsFilePath = _configuration["AppConfig:JobsFilePath"];
        }

        private JsonService _jsonService = new JsonService();
        public bool CreateJob(BackupJob backupJob)
        {

            List<BackupJob> jobs = _jsonService.GetLog<BackupJob>(_jobsFilePath) ?? new List<BackupJob>(); ;


            if (jobs.Count < 5)
            {
                if (backupJob.SourceDir == backupJob.TargetDir)
                {
                    Console.WriteLine("Le répertoire source et cible ne peuvent pas être les mêmes.");
                    return false;
                }
                backupJob.Id = jobs.Count + 1;
                if (!System.IO.Directory.Exists(backupJob.SourceDir) || !System.IO.Directory.Exists(backupJob.TargetDir))
                {
                    Console.WriteLine("Le répertoire source ou cible n'existe pas.");
                    return false;
                }
                jobs.Add(backupJob);
                _jsonService.SaveLog(jobs, _jobsFilePath);
                Console.WriteLine(String.Format(Resources.Translation.create_job_success, backupJob.Name, backupJob.Id, backupJob.SourceDir,backupJob.TargetDir,backupJob.Type));

                return true;
            }
            else
            {
                Console.WriteLine(Resources.Translation.cant_create_more_job);
                return false;
            }
        }
        public BackupJob? GetJob(int id)
        {
             List<BackupJob> jobs = _jsonService.GetLog<BackupJob>(_jobsFilePath);
             BackupJob? backupJob;

            if (jobs == null)
            {
                Console.WriteLine(String.Format(Resources.Translation.job_doesnt_exist, id));
                return null;
            }

            else
            {
                backupJob = jobs.Find(j => j.Id == id);
                if (backupJob == null)
                {
                    Console.WriteLine(String.Format(Resources.Translation.job_doesnt_exist, id));
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
            return _jsonService.GetLog<BackupJob>(_jobsFilePath);
        }

        public bool DeleteJob(int idToDelete)
        {

            List<BackupJob> jobs = _jsonService.GetLog<BackupJob>(_jobsFilePath);
            BackupJob? jobToDelete = jobs.Find(j => j.Id == idToDelete);

            if (jobToDelete != null)
            {
                jobs.Remove(jobToDelete);
                UpdateIds(jobs);
                _jsonService.SaveLog(jobs, _jobsFilePath);
                Console.WriteLine(String.Format(Resources.Translation.delete_job_success, idToDelete));
                return true;
            }
            else
            {
                Console.WriteLine(String.Format(Resources.Translation.job_doesnt_exist, idToDelete));
                return false;
            }
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
