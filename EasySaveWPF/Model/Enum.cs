﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveWPF.Model
{
    public class Enum
    {
        public enum LanguageEnum
        {
            fr,
            en,
            es,
            it,
            de
        };

        public enum JobTypeEnum
        {
            full,
            differential
        };

        public enum LogType
        {
            Json,
            Xaml
        };
    }
}
