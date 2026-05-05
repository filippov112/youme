using Pdp.Domain.Interfaces;

namespace Pdp.Domain.Models.PromptComponents.FileComponents
{
    public class FilePath(string key, string path) : IComponent
    {
        public string Key => key;
        public string Value { get; set; } = path;
    }
}
