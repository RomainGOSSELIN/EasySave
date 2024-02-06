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
        public string BackupName { get; set; }
        public  DateTime Timestamp { get; set; }
        public  string State { get; set; }
        public  int TotalFiles { get; set; }
        public  long TotalSize { get; set; }
        public  int RemainingFiles { get; set; }
        public long RemainingSize { get; set; }
        public string CurrentFile {get; set;}

    }
}
