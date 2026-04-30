using Domain.Interfaces;
using Domain.Models.Other;
using Domain.Models.PromptComponents.FileComponents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.PromptComponents
{
    public class File(string structure, FilePath path, FileContent content) : ComplexBlock(structure, [path, content]), IComponent
    {
        public string Key => "##file##";
        public string Value { get; set; } = string.Empty;
    }
}
