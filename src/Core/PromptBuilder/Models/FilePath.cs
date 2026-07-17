using Core.PromptBuilder.Interfaces;

namespace Core.PromptBuilder.Models
{
    public class FilePath(string key, string path) : IComponent
    {
        public string Key => key;
        public string Value { get; set; } = path;
    }
}
