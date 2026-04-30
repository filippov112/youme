using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.PromptComponents
{
    public class Introduction(string value) : IComponent
    {
        public string Key => "##input##";
        public string Value { get; set; } = value;
    }
}
