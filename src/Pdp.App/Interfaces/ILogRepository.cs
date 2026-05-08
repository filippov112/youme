using Pdp.App.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pdp.App.Interfaces
{
    public interface ILogRepository
    {
        public Task AddAsync(LogMessage message);
        public Task RemoveRange(DateTime? startTime = null, DateTime? endTime = null);
        public Task<IEnumerable<LogMessage>> GetAsync(DateTime? startTime = null, DateTime? endTime = null);
    }
}
