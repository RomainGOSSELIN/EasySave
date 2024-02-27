﻿using EasySaveWPF.Model;
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
    public class PauseCommand : CommandBase
    {
        private BackupViewModel _backupViewModel;

        public PauseCommand(BackupViewModel vm)
        {
            _backupViewModel = vm;
        }

        public override bool CanExecute(object? parameter)
        {
                return true;
        }

        public override void Execute(object parameter)
        {
            var job = (BackupJob)parameter;
            job.ResetEvent.Reset();
            job.State.State = Model.Enum.StateEnum.PAUSED;

        }




    }
}
