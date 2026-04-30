using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.PromptComponents.FileComponents
{
    public class FilePath(string path) : IComponent
    {
        public string Key => "##path##";
        public string Value { get; set; } = path;
    }
}
