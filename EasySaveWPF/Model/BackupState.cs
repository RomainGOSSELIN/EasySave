using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static EasySaveWPF.Model.Enum;

namespace EasySaveWPF.Model
{
    public class BackupState
    {
       
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public StateEnum State { get; set; } = StateEnum.END;
        public int TotalFilesToCopy { get; set; } = 0;
        public long TotalFilesSize { get; set; } = 0;
        public int NbFilesLeftToDo { get; set; } = 0;
        public long NbFilesSizeLeftToDo { get; set; } = 0;
        public string SourceFilePath { get; set; } = string.Empty;
        public string TargetFilePath { get; set; } = string.Empty;
        
        public int FileProgress => TotalFilesToCopy- NbFilesLeftToDo;
        public long FileSizeProgress => TotalFilesSize - NbFilesSizeLeftToDo;

        public double Progress => Math.Round((double)(TotalFilesToCopy-NbFilesLeftToDo) / TotalFilesToCopy * 100) >= 0 ? Math.Round((double)(TotalFilesToCopy - NbFilesLeftToDo) / TotalFilesToCopy * 100): 0;


        public BackupState()
        {

        }

        [JsonConstructor]
        public BackupState(DateTime timestamp, StateEnum state, int totalFilesToCopy, long totalFilesSize, int nbFilesLeftToDo, long nbFilesSizeLeftToDo, string sourceFilePath, string targetFilePath)
        {
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
