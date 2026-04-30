using Domain.Interfaces;
using Domain.Models.Other;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.PromptComponents
{
    public class Context(List<File> files) : ComplexBlock(string.Empty, [.. files.Select(f => (IComponent)f)]), IComponent
    {
        public string Key => "##project##";
        public string Value => throw new NotImplementedException();
        public override string Build()
        {
            var result = new List<string>();
            foreach (var component in files)
            {
                string value = component.Value;
                if (component is ComplexBlock complexComponent)
                    value = complexComponent.Build();
                result.Add(value);
            }
            return string.Join('\n', result);
        }
    }
}
