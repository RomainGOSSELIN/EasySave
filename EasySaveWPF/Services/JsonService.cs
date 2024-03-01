using EasySaveWPF.Model;
using EasySaveWPF.Model.LogFactory;
using EasySaveWPF.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveWPF.Services
{
    public class JsonService : ILoggerStrategy
    {
        private Notifications.Notifications _notifications = new Notifications.Notifications(); 
        public List<T> GetLog<T>(string directory)
        {
            List<T> logs = new List<T>();

            try
            {
                if (!File.Exists(directory))
                {
                    return logs;
                }

                string contenu = File.ReadAllText(directory);
                if(contenu != string.Empty)
                {
                    logs = JsonConvert.DeserializeObject<List<T>>(contenu);
                }
            }
            catch 
            {

            }

            return logs;
        }

        public void SaveLog<T>(List<T> logs, string directory)
        {
            try
            {
                string folderPath = Path.GetDirectoryName(directory);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string contenu = JsonConvert.SerializeObject(logs, Formatting.Indented);

                using (StreamWriter writer = new StreamWriter(directory))
                {
                    writer.Write(contenu);
                }
            }
            catch 
            {

            }
        }

    }

}
