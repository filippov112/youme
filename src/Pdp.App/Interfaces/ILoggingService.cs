using Pdp.App.Models;
using System.Collections.ObjectModel;

namespace Pdp.App.Interfaces
{
    public interface ILoggingService
    {
        public ObservableCollection<LogMessage> Logs { get; }

        public Task LogInfo(string message, string sender);
        public Task LogWarning(string message, string sender);
        public Task LogError(string message, string sender);
        public Task LogDebug(string message, string sender);

        public Task ClearData(DateTime? startTime = null, DateTime? endTime = null);
        public Task GetData(DateTime? startTime = null, DateTime? endTime = null);
    }
}
