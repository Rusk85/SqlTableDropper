using System.Collections.Generic;
using System;

namespace CavemanTools.Data
{
    public class JsonStruct
    {
        public const string MessageKey = "Message";
        public string Status { get; set; }
        public Dictionary<string, object> Data { get; private set; }
      

        public object this[string key]
        {
            get { return Data[key]; }
            set { Data[key] = value; }
        }

        /// <summary>
        /// Sets the status and inserts a message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public JsonStruct(string message,JsonStatus status = JsonStatus.Ok):this(status)
        {
            this[MessageKey] = message;
        }
        public JsonStruct(JsonStatus status=JsonStatus.Ok)
        {
            Status = status.ToString();
            Data= new Dictionary<string, object>();            
        }
    }

}