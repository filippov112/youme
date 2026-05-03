using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IPromptBuilder
    {
        public Task<string> GetPrompt(List<string> filePath, string queryText);
    }
}
