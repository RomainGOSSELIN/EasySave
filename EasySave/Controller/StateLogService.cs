using EasySave.Controller.Interfaces;
using EasySave.Model;
using EasySave.Controller;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Controller
{
    public class StateLogService : IStateLogService

    {
        private readonly IConfiguration _configuration;
        private readonly JsonService _jsonService = new JsonService();
        private string _stateLogPath;
        public StateLogService(IConfiguration configuration)
        {
            _configuration = configuration;
            _stateLogPath = _configuration["AppConfig:StatusFilePath"];
        }
        public void SaveStateLog(string state, string directory)
        {
            string contenu = state;
            System.IO.File.WriteAllText(directory, contenu);
        }
        public void CreateStateLog(BackupJob job)
        {

            List<BackupState> state = _jsonService.GetLog<BackupState>(_stateLogPath);

            var newstate = new BackupState(job.Id, job.Name, DateTime.Now,"END",0,0,0,0,"","");

            state.Add(newstate);

            _jsonService.SaveLog(state, _stateLogPath);

        }

        public void UpdateStateLog(BackupJob job)
        {

            //List<BackupState> state = _jsonService.GetLog<BackupState>(_stateLogPath);

            //var newstate = new BackupState(job.Id, job.Name, DateTime.Now, "END", 0, 0, 0, 0, "", "");

            //state.Add(newstate);

            //_jsonService.SaveLog(state, _stateLogPath);

        }

        public void DeleteStateLog(int idToDelete) {

            List<BackupState> state = _jsonService.GetLog<BackupState>(_stateLogPath);

            state.RemoveAll(x => x.Id == idToDelete);

            UpdateIds(state);

            _jsonService.SaveLog(state, _stateLogPath);
            Console.WriteLine("DeleteStateLog");
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
