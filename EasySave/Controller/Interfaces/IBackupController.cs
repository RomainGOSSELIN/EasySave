using ConsoleTables;
using System.Reflection.Metadata;
using static EasySave.Model.Enum;
using Spectre.Console;

namespace EasySave.Controller
{
    public interface IBackupController
    {
        void ExecuteJob(string id);
        void CreateJob(string jobName, string source, string dest, JobTypeEnum type);
        void DeleteJob(string idToDelete);
        Table ShowJob(string id, bool all);
    }
}
