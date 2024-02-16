using EasySaveWPF.Model;
using EasySaveWPF.Model.LogFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveWPF.Services
{
    public class JsonService : ILogger
    {
        public List<T> GetLog<T>(string directory)
        {
            List<T> logs = new List<T>();

            if (File.Exists(directory))
            {
                string contenu = File.ReadAllText(directory);
                logs = JsonConvert.DeserializeObject<List<T>>(contenu);
            }

            return logs;
        }

        public void SaveLog<T>(List<T> logs, string directory)
        {
            string contenu = JsonConvert.SerializeObject(logs, Formatting.Indented);

            using (StreamWriter writer = new StreamWriter(directory))
            {
                writer.Write(contenu);
            }
        }

    }

}
