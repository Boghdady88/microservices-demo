using Newtonsoft.Json;
using System;
namespace Center.Core.Models
{
    public class LogDataDto
    {
        public string ServiceName { get; set; }
        public string LogData { get; set; }

        public string ExceptionData { get; set; }
        public LogTypes LogType { get; set; } = LogTypes.Information;
        

        public object GetException()
        {
            var result = JsonConvert.DeserializeObject(ExceptionData); 
            return result;
        }
    }
}
