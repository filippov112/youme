using Core.PromptBuilder.Interfaces;

namespace Core.PromptBuilder.Models
{
    public class FileContent(string key, string content) : IComponent
    {
        public string Key => key;
        public string Value { get; set; } = content;
    }
}
