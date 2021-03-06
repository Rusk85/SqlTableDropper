namespace CavemanTools.Logging
{
    public abstract class LogWriterBase:IWriteToLog
    {
        public abstract T GetLogger<T>() where T:class ;
        public abstract void Log(LogLevel level, string text);


        public abstract void Log(LogLevel level, string message, params object[] args);

        public void Info(string text)
        {
            Log(LogLevel.Info,text);
        }

        public void Info(string format, params object[] args)
        {
            Log(LogLevel.Info, format,args);
        }

       
        
        public void Trace(string message)
        {
            Log(LogLevel.Trace, message);
        }

        public void Trace(string format, params object[] args)
        {
            Log(LogLevel.Trace, format, args);
        }

        public void Error(string text)
        {
            Log(LogLevel.Error, text);
        }

        public void Error(string format, params object[] args)
        {
            Log(LogLevel.Error, format, args);
        }

        public void Warning(string text)
        {
            Log(LogLevel.Warn, text);
        }

        public void Warning(string format, params object[] args)
        {
            Log(LogLevel.Warn, format, args);
        }

        public void Debug(string mesage)
        {
            Log(LogLevel.Debug, mesage); 
        }

        public void Debug(string format, params object[] args)
        {
            Log(LogLevel.Debug, format, args);
        }
    }
}