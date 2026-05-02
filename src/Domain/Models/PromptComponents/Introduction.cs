using Domain.Interfaces;

namespace Domain.Models.PromptComponents
{
    public class Introduction(string key, string value) : IComponent
    {
        public string Key => key;
        public string Value { get; set; } = value;
    }
}
