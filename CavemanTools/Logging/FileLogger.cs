using System;
using System.IO;

namespace CavemanTools.Logging
{
    public class FileLogger:LogWriterBase
    {
        private readonly string _filename;

        public FileLogger(string filename)
        {
            _filename = filename;
        }


        /// <summary>
        /// Should return the real logger implementation
        /// </summary>
        /// <typeparam name="T">Logger type</typeparam>
        /// <returns/>
        public override T GetLogger<T>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a log entry with the specified logging level
        /// </summary>
        /// <param name="level">Status</param><param name="text">Entry Text</param>
        public override void Log(LogLevel level, string text)
        {
            var data=(DateTime.Now.ToString() + " - " + level.ToString() + ": " + text);
            File.AppendAllText(_filename,data);
        }

        /// <summary>
        /// Writes a formatted log entry with the specified logging level
        /// </summary>
        /// <param name="level">Status</param><param name="message">Entry Text</param><param name="args">List of arguments</param>
        public override void Log(LogLevel level, string message, params object[] args)
        {
            var data = (DateTime.Now.ToString() + " - " + level.ToString() + ": " + message.ToFormat(args));
            File.AppendAllText(_filename, data); 
        }
    }
}