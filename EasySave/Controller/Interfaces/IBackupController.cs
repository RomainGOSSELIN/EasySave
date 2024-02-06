using ConsoleTables;
using static EasySave.Model.Enum;

namespace EasySave.ViewModel
{
    public interface IBackupController
    {
        void ExecuteJob(string id);
        void CreateJob(string jobName, string source, string dest, JobTypeEnum type);
        void DeleteJob(string idToDelete);
        ConsoleTable ShowJob(string id, bool all);
    }
}
