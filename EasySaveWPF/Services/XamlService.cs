namespace EasySaveWPF.Services
{
    using EasySaveWPF.Model.LogFactory;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Markup;
    using System.Xml.Serialization;

    public class XamlService : ILoggerStrategy
    {
        private Notifications.Notifications _notifications = new Notifications.Notifications();
        public List<T> GetLog<T>(string directory)
        {
            List<T> logs = new List<T>();

            try
            {
                if (File.Exists(directory) && new FileInfo(directory).Length > 0)
                {
                    using (FileStream fs = new FileStream(directory, FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                        logs = (List<T>)serializer.Deserialize(fs);
                    }
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

                using (FileStream fs = new FileStream(directory, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
                    serializer.Serialize(fs, logs);
                }
            }
            catch 
            {

            }
        }
    }
}
