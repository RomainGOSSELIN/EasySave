using EasySave.Model.LogFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace EasySave.Controller
{
    public class XamlService : ILogger
    {
        public List<T> GetLog<T>(string directory)
        {
            List<T> logs = new List<T>();

            if (File.Exists(directory))
            {
                using (FileStream fs = new FileStream(directory, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                    logs = (List<T>)serializer.Deserialize(fs);
                }
            }

            return logs;
        }

        public void SaveLog<T>(List<T> logs, string directory)
        {
            using (FileStream fs = new FileStream(directory, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                serializer.Serialize(fs, logs);
            }
        }
    }
}
