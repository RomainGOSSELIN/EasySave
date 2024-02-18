using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EasySaveWPF.Model;
using EasySaveWPF.Services.Interfaces;
using System;
using EasySaveWPF.Model.LogFactory;

namespace EasySaveWPF.Services
{


        public class BackupJobService : IBackupJobService
        {
            private static string _jobsFilePath;
            private static ILogger _logger;
            private static LoggerFactory loggerFactory = new LoggerFactory();
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
                        Console.WriteLine(Resources.Translation.target_source_must_be_different);
                        return false;
                    }
                    backupJob.Id = jobs.Count + 1;
                    if (!System.IO.Directory.Exists(backupJob.SourceDir))
                    {
                        Console.WriteLine(Resources.Translation.source_directory_doesnt_exist);
                        return false;
                    }
                    jobs.Add(backupJob);
                    _logger.SaveLog(jobs, _jobsFilePath);
                    Console.WriteLine(String.Format(Resources.Translation.create_job_success, backupJob.Name, backupJob.Id, backupJob.SourceDir, backupJob.TargetDir, backupJob.Type));

                    return true;
                
     
            }
            public BackupJob? GetJob(int id)
            {
                List<BackupJob> jobs = _logger.GetLog<BackupJob>(_jobsFilePath);
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

            private void UpdateIds(List<BackupJob> jobs)
            {
                for (int i = 0; i < jobs.Count; i++)
                {
                    jobs[i].Id = i + 1;
                }
            }



    }

}
