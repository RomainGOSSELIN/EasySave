using EasySave.Controller.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasySave.Model.Enum;

namespace EasySave.Controller
{
    public class SettingsService : ISettingsService
    {
        public void ChangeOptions(LanguageEnum language, LogTypeEnum logtype)
        {
            string filePath = ".\\appsettings.json";

            string json = File.ReadAllText(filePath);

            JObject jsonObj = JObject.Parse(json);

            jsonObj["AppConfig"]["Language"] = Enum.GetName(language);
            jsonObj["AppConfig"]["LogType"] = Enum.GetName(logtype);
            jsonObj["AppConfig"]["LogFilePath"] = logtype == LogTypeEnum.json ? ".\\Log.json" : ".\\Log.xml";

            File.WriteAllText(filePath, jsonObj.ToString());
        }
    }
}
