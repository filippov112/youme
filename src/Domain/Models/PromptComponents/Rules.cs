using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.PromptComponents
{
    public class Rules(string value) : IComponent
    {
        public string Key => "##settings##";
        public string Value { get; set; } = value;
    }
}
