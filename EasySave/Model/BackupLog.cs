using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Model
{
    internal class BackupLog
    {
        public  DateTime timestamp;
        public string BackupName;
        public  string sourceFile;
        public  string targetFile;
        public  long fileSize;
        public  int transferTime;
        public  string Type { get; set; }
    }
}
