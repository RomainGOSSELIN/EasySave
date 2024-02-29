using ClientWPFConsole.Services;
using ClientWPFConsole.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            if (_mainViewModel.ClientService == null || !_mainViewModel.ClientService.IsConnected)
            {
                if (_mainViewModel.ServerIp == string.Empty)
                {
                    MessageBox.Show("Please enter a server IP");
                }
                else
                {
                    _mainViewModel.ClientService.SetIp(_mainViewModel.ServerIp);
                    _mainViewModel.ClientService.Connect();
                }
            }
            else
            {
                _mainViewModel.ClientService.Disconnect();
            }
        }
    }

}
