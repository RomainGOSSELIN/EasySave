using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasySaveWPF.Model.Enum;

namespace EasySaveWPF.Model
{
    public class BackupJob
    {
        public int? Id { get; set; } = 0;
        public  string Name { get; set; } = string.Empty;
        public string SourceDir { get; set; } = string.Empty;
        public string TargetDir { get; set; } = string.Empty;
        public JobTypeEnum Type { get; set; } = JobTypeEnum.differential;

        public BackupState State { get; set; } = new BackupState();

        public BackupJob() 
        {

        }
        public BackupJob(string name, string sourceDir, string targetDir, JobTypeEnum type, int id, BackupState state) {

            Id = id;
            Name = name;
            SourceDir = sourceDir;
            TargetDir = targetDir;
            Type = type;
            State = state;
    }

    }
}
