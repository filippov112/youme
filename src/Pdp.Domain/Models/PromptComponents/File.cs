using Pdp.Domain.Interfaces;
using Pdp.Domain.Models.Other;
using Pdp.Domain.Models.PromptComponents.FileComponents;

namespace Pdp.Domain.Models.PromptComponents
{
    public class File(string structure, FilePath path, FileContent content) : ComplexBlock(structure, [path, content]), IComponent
    {
        public string Key => string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
