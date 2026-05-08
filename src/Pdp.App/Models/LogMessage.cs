namespace Pdp.App.Models
{
    public class LogMessage
    {
        public DateTime Timestamp { get; set; }
        public LogMessageType Type { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;
        public DateTime SessionTime { get; set; }

        public LogMessage(DateTime sessionTime, LogMessageType type, string message, string sender)
        {
            Timestamp = DateTime.Now;
            Type = type;
            Message = message;
            Sender = sender;
            SessionTime = sessionTime;
        }
    }
    public enum LogMessageType
    {
        Info,
        Warning,
        Error,
        Debug
    }
}
