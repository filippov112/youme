using Pdp.Domain.Models.LastProjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Pdp.App.Interfaces
{
    public interface IRecentProjectsService
    {
        ObservableCollection<ProjectInfo> RecentProjects { get; }
        void AddProject(string projectPath, string projectName);
        void RemoveProject(string projectPath);
        void ClearAll();
        void LoadFromStorage();
        void SaveToStorage();
        int MaxCount { get; set; }
    }
}
