using System;
using Newtonsoft.Json;

namespace Center.Core.Models
{
    public enum LogTypes
    {
        Verbose = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5
    }

    public class LogDataModel
    {
        public string ServiceName { get; private set; }
        public string LogData { get; private set; }
        public string ExceptionData { get; private set; }
        public LogTypes LogType { get; private set; }
        public DateTime LogTime { get; private set; }


        public static LogDataModel CreateLogData(LogDataDto data , Exception exception = null)
        {
            return new LogDataModel
            {
                ServiceName = data.ServiceName,
                LogData = data.LogData,
                LogType = data.LogType,
                LogTime = DateTime.Now,
                ExceptionData = JsonConvert.SerializeObject(exception),
            };
        }
    }

}
