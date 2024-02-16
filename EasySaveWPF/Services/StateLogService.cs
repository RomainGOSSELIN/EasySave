using EasySaveWPF.Model;
using EasySaveWPF.Model.LogFactory;
using EasySaveWPF.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace EasySaveWPF.Services
{
    public class StateLogService : IStateLogService

    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private string _stateLogPath;
        public StateLogService(IConfiguration configuration)
        {
            _configuration = configuration;
            _stateLogPath = _configuration["AppConfig:StatusFilePath"];
            LoggerFactory loggerFactory = new LoggerFactory();  
            _logger = loggerFactory.CreateLogger((Model.Enum.LogType)int.Parse(_configuration["AppConfig:LogType"]));
        }
        public void SaveStateLog(string state, string directory)
        {
            string contenu = state;
            System.IO.File.WriteAllText(directory, contenu);
        }
        public void CreateStateLog(BackupJob job)
        {

            List<BackupState> state = _logger.GetLog<BackupState>(_stateLogPath);


            DateTime today = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), CultureInfo.InvariantCulture);

            var newstate = new BackupState(job.Id, job.Name, today, "END",0,0,0,0,"","");

            state.Add(newstate);

            _logger.SaveLog(state, _stateLogPath);

        }

        public void DeleteStateLog(BackupJob job) {

            List<BackupState> state = _logger.GetLog<BackupState>(_stateLogPath);

            state.RemoveAll(x => x.Id == job.Id);

            UpdateIds(state);

            _logger.SaveLog(state, _stateLogPath);
        }

        public void UpdateStateLog(BackupState state)
        {
            List<BackupState> states = _logger.GetLog<BackupState>(_stateLogPath);

            var stateToUpdate = states.FirstOrDefault(x => x.Id == state.Id);

            if (stateToUpdate != null)
            {
                //stateToUpdate.Timestamp = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss",CultureInfo.InvariantCulture));
                stateToUpdate.State = state.State;
                stateToUpdate.NbFilesLeftToDo = state.NbFilesLeftToDo;
                stateToUpdate.NbFilesSizeLeftToDo = state.NbFilesSizeLeftToDo;
                stateToUpdate.TotalFilesToCopy = state.TotalFilesToCopy;
                stateToUpdate.TotalFilesSize = state.TotalFilesSize;
                stateToUpdate.SourceFilePath = state.SourceFilePath;
                stateToUpdate.TargetFilePath = state.TargetFilePath;
            }

            _logger.SaveLog(states, _stateLogPath);
        }

        private void UpdateIds(List<BackupState> states)
        {
            for (int i = 0; i < states.Count; i++)
            {
                states[i].Id = i + 1;
            }
        }

    }
}
