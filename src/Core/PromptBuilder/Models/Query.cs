using Core.PromptBuilder.Interfaces;

namespace Core.PromptBuilder.Models
{
    public class Query(string key, string value) : IComponent
    {
        public string Key => key;
        public string Value { get; set; } = value;
    }
}
