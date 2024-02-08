using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasySave.Model.Enum;

namespace EasySave.Model
{
    public class BackupJob
    {
        public  int? Id { get; set; }
        public  string Name { get; set; }
        public  string SourceDir { get; set; }
        public  string TargetDir { get; set; }
        public  JobTypeEnum Type { get; set; }

        public BackupJob(string name, string sourceDir, string targetDir, JobTypeEnum type, int? id = 0) {

            Id = id;
            Name = name;
            SourceDir = sourceDir;
            TargetDir = targetDir;
            Type = type;
    }

    }
}
