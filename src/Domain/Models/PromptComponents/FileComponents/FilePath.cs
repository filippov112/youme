using Domain.Interfaces;

namespace Domain.Models.PromptComponents.FileComponents
{
    public class FilePath(string key, string path) : IComponent
    {
        public string Key => key;
        public string Value { get; set; } = path;
    }
}
