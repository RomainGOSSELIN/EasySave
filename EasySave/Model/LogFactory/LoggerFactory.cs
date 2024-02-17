using EasySave.Controller;
using Microsoft.Extensions.Logging;
using Portable.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasySave.Model.Enum;

namespace EasySave.Model.LogFactory
{
    public class LoggerFactory
    {
        public static ILogger CreateLogger(LogTypeEnum logType)
        {
            switch (logType)
            {
                case LogTypeEnum.json:
                    return new JsonService();
                case LogTypeEnum.xaml:
                    return new XamlService();
                default:
                    throw new ArgumentException("Type de log non pris en charge.");
            }
        }

    }
}
