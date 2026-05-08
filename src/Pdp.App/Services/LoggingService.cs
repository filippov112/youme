using Pdp.App.Interfaces;
using Pdp.App.Models;
using System.Collections.ObjectModel;

namespace Pdp.App.Services
{
    public class LoggingService(ILogRepository logRepository) : ILoggingService
    {
        public ObservableCollection<LogMessage> Logs { get; } = [];

        private readonly Lock _lock = new();
        private readonly DateTime _sessionStartTime = DateTime.Now;

        private async Task Log(LogMessageType type, string message, string sender)
        {
            var logMessage = new LogMessage(_sessionStartTime, type, message, sender);
            lock (_lock)
            {
                Logs.Add(logMessage);
                logRepository.AddAsync(logMessage).Wait();
            }
        }


        public async Task LogInfo(string message, string sender) => await Log(LogMessageType.Info, message, sender);
        public async Task LogWarning(string message, string sender) => await Log(LogMessageType.Warning, message, sender);
        public async Task LogError(string message, string sender) => await Log(LogMessageType.Error, message, sender);
        public async Task LogDebug(string message, string sender) => await Log(LogMessageType.Debug, message, sender);


        public async Task ClearData(DateTime? startTime = null, DateTime? endTime = null)
        {
            Logs.Clear();
            await logRepository.RemoveRange(startTime, endTime);
        }

        public async Task GetData(DateTime? startTime = null, DateTime? endTime = null)
        {
            Logs.Clear();
            var logs = await logRepository.GetAsync(startTime, endTime);
            foreach(var log in logs)
            {
                Logs.Add(log);
            }
        }
    }
}
