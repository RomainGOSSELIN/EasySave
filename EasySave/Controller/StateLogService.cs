using EasySave.Controller.Interfaces;
using EasySave.Model;
using EasySave.Model.LogFactory;
using EasySave.Controller;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasySave.Model.Enum;
using System.Globalization;

namespace EasySave.Controller
{
    public class StateLogService : IStateLogService

    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logService;
        private readonly string _stateLogPath;
        private readonly LogTypeEnum logType;

        public StateLogService(IConfiguration configuration)
        {
            _configuration = configuration;
            _stateLogPath = _configuration["AppConfig:StatusFilePath"];
            _logService = LoggerFactory.CreateLogger(logType);
        }

        public void SaveStateLog(string state, string directory)
        {
            string contenu = state;
            System.IO.File.WriteAllText(directory, contenu);
        }
        public void CreateStateLog(BackupJob job)
        {

            List<BackupState> state = _logService.GetLog<BackupState>(_stateLogPath);

            var newstate = new BackupState(job.Id, job.Name, DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), CultureInfo.InvariantCulture), "END",0,0,0,0,"","");

            state.Add(newstate);

            _logService.SaveLog(state, _stateLogPath);

        }

        public void DeleteStateLog(int idToDelete) {

            List<BackupState> state = _logService.GetLog<BackupState>(_stateLogPath);

            state.RemoveAll(x => x.Id == idToDelete);

            UpdateIds(state);

            _logService.SaveLog(state, _stateLogPath);
        }

        public void UpdateStateLog(BackupState state)
        {
            List<BackupState> states = _logService.GetLog<BackupState>(_stateLogPath);

            var stateToUpdate = states.FirstOrDefault(x => x.Id == state.Id);

            if (stateToUpdate != null)
            {
                stateToUpdate.Timestamp = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), CultureInfo.InvariantCulture);
                stateToUpdate.State = state.State;
                stateToUpdate.NbFilesLeftToDo = state.NbFilesLeftToDo;
                stateToUpdate.NbFilesSizeLeftToDo = state.NbFilesSizeLeftToDo;
                stateToUpdate.TotalFilesToCopy = state.TotalFilesToCopy;
                stateToUpdate.TotalFilesSize = state.TotalFilesSize;
                stateToUpdate.SourceFilePath = state.SourceFilePath;
                stateToUpdate.TargetFilePath = state.TargetFilePath;
            }

            _logService.SaveLog(states, _stateLogPath);
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
