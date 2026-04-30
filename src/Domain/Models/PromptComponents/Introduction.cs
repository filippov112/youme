using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.PromptComponents
{
    public class Introduction(string key, string value) : IComponent
    {
        public string Key => key;
        public string Value { get; set; } = value;
    }
}
