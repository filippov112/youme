using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Selections.Models
{
    public class Selection
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Files { get; set; } = [];
    }
}
