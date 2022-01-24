namespace Center.Core.Models
{
    public class LogDataDto
    {
        public string ServiceName { get; set; }
        public string LogData { get; set; }
        public LogTypes LogType { get; set; } = LogTypes.Information;
    }
}
