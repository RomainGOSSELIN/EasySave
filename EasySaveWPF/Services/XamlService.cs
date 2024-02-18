namespace EasySaveWPF.Services
{
    using EasySaveWPF.Model.LogFactory;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Markup;
    using System.Xml.Serialization;

    public class XamlService : ILogger
    {
        public List<T> GetLog<T>(string directory)
        {
            List<T> logs = new List<T>();

            if (File.Exists(directory) && new FileInfo(directory).Length > 0)
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
