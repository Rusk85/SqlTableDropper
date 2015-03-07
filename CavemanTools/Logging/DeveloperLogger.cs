using System;

namespace CavemanTools.Logging
{
    public class DeveloperLogger:LogWriterBase
    {
        private Action<string> _writer;

        public override T GetLogger<T>()
        {
            throw new System.NotImplementedException();
        }

        public DeveloperLogger(Action<string> writer)
        {
            _writer = writer;
        }

        public override void Log(LogLevel level, string text)
        {
            _writer(DateTime.Now.ToString() + " - " + level.ToString() + ": " + text);

        }

        public override void Log(LogLevel level, string message, params object[] args)
        {
            _writer(DateTime.Now.ToString() + " - " + level.ToString() + ": " + string.Format(message,args));            
        }
    }
}