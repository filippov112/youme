using Domain.Interfaces;


namespace Domain.Models.PromptComponents.FileComponents
{
    public class FileContent(string key, string content) : IComponent
    {
        public string Key => key;
        public string Value { get; set; } = content;
    }
}
