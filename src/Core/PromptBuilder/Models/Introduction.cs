using Core.PromptBuilder.Interfaces;

namespace Core.PromptBuilder.Models
{
    public class Introduction(string key, string value) : IComponent
    {
        public string Key => key;
        public string Value { get; set; } = value;
    }
}
