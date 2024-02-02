using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Model
{
    internal class BackupState
    {
        public  DateTime timestamp;
        public  string backupName;
        public  int totalFiles;
        public  long totalSize;
        public  string Type { get; set; }

    }
}
