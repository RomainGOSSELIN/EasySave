using EasySaveWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;
using static EasySaveWPF.Model.Enum;

namespace EasySaveWPF.Model.LogFactory
{
    public class LoggerFactory
    {
        public ILogger CreateLogger(LogType logType)
        {
            switch (logType)
            {
                case LogType.Json:
                    return new JsonService();
                case LogType.Xaml:
                    return new XamlService();
                default:
                    throw new ArgumentException("Type de log non pris en charge.");
            }
        }
    }
}