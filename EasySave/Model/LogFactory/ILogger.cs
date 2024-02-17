﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EasySave.Model.LogFactory
{
    public interface ILogger
    {
        List<T> GetLog<T>(string directory);
        void SaveLog<T>(List<T> logs, string directory);
    }
}
