using EasySave.Model.LogFactory;
using Newtonsoft.Json;

namespace EasySave.Controller
{
    public class JsonService : ILogger
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
