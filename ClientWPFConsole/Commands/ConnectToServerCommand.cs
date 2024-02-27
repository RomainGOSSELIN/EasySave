using ClientWPFConsole.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPFConsole.Commands
{
    public class ConnectToServerCommand : CommandBase
    {
        private readonly MainViewModel _mainViewModel;

        public ConnectToServerCommand(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public override bool CanExecute(object? parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            if (!_mainViewModel.ClientService.IsConnected)
            {

                _mainViewModel.ClientService.Connect();
            }
            else
            {
                _mainViewModel.ClientService.Disconnect();
            }
        }
    }

}
