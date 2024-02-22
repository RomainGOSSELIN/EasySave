using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EasySaveWPF.Model
{
    public class BackupState
    {
        public int? Id { get; set; } = 0;
        public string BackupName { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string State { get; set; } = "END";
        public int TotalFilesToCopy { get; set; } = 0;
        public long TotalFilesSize { get; set; } = 0;
        public int NbFilesLeftToDo { get; set; } = 0;
        public long NbFilesSizeLeftToDo { get; set; } = 0;
        public string SourceFilePath { get; set; } = string.Empty;
        public string TargetFilePath { get; set; } = string.Empty;
        public double Progress => Math.Round((double)(TotalFilesToCopy-NbFilesLeftToDo) / TotalFilesToCopy * 100) >= 0 ? Math.Round((double)(TotalFilesToCopy - NbFilesLeftToDo) / TotalFilesToCopy * 100): 0;

        public BackupState()
        {

        }

        [JsonConstructor]
        public BackupState(int? id, string backupName, DateTime timestamp, string state, int totalFilesToCopy, long totalFilesSize, int nbFilesLeftToDo, long nbFilesSizeLeftToDo, string sourceFilePath, string targetFilePath)
        {
            Id = id;
            BackupName = backupName;
            Timestamp = timestamp;
            State = state;
            TotalFilesToCopy = totalFilesToCopy;
            TotalFilesSize = totalFilesSize;
            NbFilesLeftToDo = nbFilesLeftToDo;
            NbFilesSizeLeftToDo = nbFilesSizeLeftToDo;
            SourceFilePath = sourceFilePath;
            TargetFilePath = targetFilePath;
        }
    }
}
