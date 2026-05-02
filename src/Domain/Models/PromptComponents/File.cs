using Domain.Interfaces;
using Domain.Models.Other;
using Domain.Models.PromptComponents.FileComponents;

namespace Domain.Models.PromptComponents
{
    public class File(string structure, FilePath path, FileContent content) : ComplexBlock(structure, [path, content]), IComponent
    {
        public string Key => string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
