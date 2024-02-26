using EasySaveWPF.Model;
using EasySaveWPF.Services;
using EasySaveWPF.Services.Interfaces;
using EasySaveWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasySaveWPF.Commands
{
    public class SaveSettingsCommand : CommandBase
    {

        public SaveSettingsCommand()
        {

        }

        public override bool CanExecute(object? parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            Properties.Settings.Default.Save();


        }




    }
}
