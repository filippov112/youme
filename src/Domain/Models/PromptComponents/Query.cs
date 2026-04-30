using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.PromptComponents
{
    public class Query(string value) : IComponent
    {
        public string Key => "##request##";
        public string Value { get; set; } = value;
    }
}
