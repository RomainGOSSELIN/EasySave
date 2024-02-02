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
        public required DateTime timestamp;
        public required string backupName;
        public required int totalFiles;
        public required long totalSize;
        public required string Type { get; set; }



    }
}
