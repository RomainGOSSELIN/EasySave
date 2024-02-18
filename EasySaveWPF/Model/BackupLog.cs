using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveWPF.Model
{
    public class BackupLog
    {
    

        public  DateTime timestamp { get; set; }
        public string BackupName { get; set; }
        public  string SourceFile { get; set; }
        public  string TargetFile { get; set; }
        public  long FileSize { get; set; }
        public  int TransferTime { get; set; }

        public BackupLog()
        {

        }

        public BackupLog(string backupName, DateTime timestamp, string sourceFile, string targetFile, long fileSize, int transferTime)
        {
            this.timestamp = timestamp;
            this.BackupName = backupName;
            this.SourceFile = sourceFile;
            this.TargetFile = targetFile;
            this.FileSize = fileSize;
            this.TransferTime = transferTime;
        }
    }
}
