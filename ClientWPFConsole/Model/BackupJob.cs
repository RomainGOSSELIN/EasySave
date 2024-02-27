using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasySaveWPF.Model.Enum;

namespace ClientWPFConsole.Model
{
    public class BackupJob
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string SourceDir { get; set; } = string.Empty;
        public string TargetDir { get; set; } = string.Empty;
        public JobTypeEnum Type { get; set; } = JobTypeEnum.differential;
        public BackupState State { get; set; } = new BackupState();



    }

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

        public int FileProgress => TotalFilesToCopy - NbFilesLeftToDo;
        public long FileSizeProgress => TotalFilesSize - NbFilesSizeLeftToDo;

        public double Progress => Math.Round((double)(TotalFilesToCopy - NbFilesLeftToDo) / TotalFilesToCopy * 100) >= 0 ? Math.Round((double)(TotalFilesToCopy - NbFilesLeftToDo) / TotalFilesToCopy * 100) : 0;


    }
}
