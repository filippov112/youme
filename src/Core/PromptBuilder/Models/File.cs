using Core.PromptBuilder.Interfaces;

namespace Core.PromptBuilder.Models
{
    public class File(string structure, FilePath path, FileContent content) : ComplexBlock(structure, [path, content]), IComponent
    {
        public string Key => string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
