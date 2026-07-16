using System;
using System.Collections.Generic;
using System.Text;

namespace Pdp.Domain.Models.LastProjects
{
    public class ProjectInfo
    {
        public string ProjectPath { get; set; }
        public string ProjectName { get; set; }
        public DateTime LastOpened { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ProjectInfo other && ProjectPath == other.ProjectPath;
        }

        public override int GetHashCode()
        {
            return ProjectPath?.GetHashCode() ?? 0;
        }
    }
}
