using Pdp.Domain.Interfaces;

namespace Pdp.Domain.Models.PromptComponents
{
    public class Introduction(string key, string value) : IComponent
    {
        public string Key => key;
        public string Value { get; set; } = value;
    }
}
