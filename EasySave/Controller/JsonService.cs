using EasySave.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.ViewModel
{
    internal class JsonService
    {
        public List<T> GetLog<T>(string directory)
        {
            List<T> logs = new List<T>();

            if(File.Exists(directory))
            {
                string contenu = File.ReadAllText(directory);
                logs = JsonConvert.DeserializeObject<List<T>>(contenu);
            }


            return logs;
        }
        
        public void SaveLog<T>(List<T> logs, string directory)
        {
            string contenu = JsonConvert.SerializeObject(logs, Formatting.Indented) ;
            File.WriteAllText(directory, contenu);
        }

    }
}
