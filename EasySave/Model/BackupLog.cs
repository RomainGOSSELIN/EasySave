using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Model
{
    internal class BackupLog
    {
        public required DateTime timestamp;
        public required string backupName;
        public required string sourceFile;
        public required string targetFile;
        public required long fileSize;
        public required int transferTime;
        public required string Type { get; set; }
    }
}
