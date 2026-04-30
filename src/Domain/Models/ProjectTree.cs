using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class ProjectTree
    {
        public ProjectUnit Root { get; set; } = new();
    }
}
