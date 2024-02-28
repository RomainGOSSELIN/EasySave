
    namespace EasySaveWPF.Model.LogFactory
    {
        public class LoggerContext
        {
            private ILoggerStrategy _strategy;

            public LoggerContext()
            { 
            }

            public LoggerContext(ILoggerStrategy strategy)
            {
                _strategy = strategy;
            }
            public void SetStrategy(ILoggerStrategy strategy)
            {
                this._strategy = strategy;
            }
            public void Save<T>(List<T> logs, string directory)
            {
                _strategy.SaveLog<T>(logs,directory);
            }

            public List<T> Get<T>(string dir)
            {
               return _strategy.GetLog<T>(dir);
            }

        
        }
    }