namespace EasySaveWPF.Services
{
    using EasySaveWPF.Model.LogFactory;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Markup;

    internal class XamlService : ILogger
    {
        public List<T> GetLog<T>(string directory)
        {
            List<T> logs = new List<T>();

            if (File.Exists(directory))
            {
                using (FileStream fs = new FileStream(directory, FileMode.Open))
                {
                    logs = (List<T>)XamlReader.Load(fs);
                }
            }

            return logs;
        }

        public void SaveLog<T>(List<T> logs, string directory)
        {
            using (FileStream fs = new FileStream(directory, FileMode.Create))
            {
                XamlWriter.Save(logs, fs);
            }
        }



    }
}
