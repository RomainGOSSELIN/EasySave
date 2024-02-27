using ClientWPFConsole.Model;
using ClientWPFConsole.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientWPFConsole.Commands
{
    public class StopJobCommand : CommandBase
    {


        private MainViewModel _mainViewModel;

        public StopJobCommand(MainViewModel vm)
        {
            _mainViewModel = vm;
        }

        public override bool CanExecute(object? parameter)
        {
            return true;
        }

        public override async void Execute(object parameter)
        {
           var job = (BackupJob)parameter;

            _mainViewModel.ClientService.SendDataToServer("stop", job);


        }
    }
}
