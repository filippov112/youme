using System;
using System.Collections.Generic;
using System.Text;
using Pdp.App.Interfaces;
using Pdp.Domain.Models.LastProjects;
using Pdp.UI.Other;
// ViewModels/RecentProjectsViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace Pdp.UI.ViewModels
{
 
    public class RecentProjectsVM : ViewModel
    {
        private readonly IRecentProjectsService _recentProjectsService;
        private ProjectInfo _selectedProject;
        private bool _isVisible;

        public RecentProjectsVM(IRecentProjectsService recentProjectsService)
        {
            _recentProjectsService = recentProjectsService ?? throw new ArgumentNullException(nameof(recentProjectsService));

            RecentProjects = _recentProjectsService.RecentProjects;

            // Команды
            OpenProjectCommand = new RelayCommand<ProjectInfo>(OpenProject, CanOpenProject);
            RemoveProjectCommand = new RelayCommand<ProjectInfo>(RemoveProject, CanRemoveProject);
            ClearAllCommand = new RelayCommand(ClearAll, CanClearAll);
        }

        public ObservableCollection<ProjectInfo> RecentProjects { get; }

        public ProjectInfo SelectedProject
        {
            get => _selectedProject;
            set
            {
                if (SetProperty(ref _selectedProject, value) && value != null)
                {
                    OpenProject(value);
                }
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        private bool SetProperty<T>(ref T p, T value)
        {
            p = value;
            OnPropertyChanged(nameof(p));
            return true;
        }

        public ICommand OpenProjectCommand { get; }
        public ICommand RemoveProjectCommand { get; }
        public ICommand ClearAllCommand { get; }

        // Событие, которое будет обрабатывать главное окно
        public event EventHandler<ProjectOpenedEventArgs> ProjectOpened;

        private void OpenProject(ProjectInfo? project)
        {
            if (project == null) return;

            // Обновляем время открытия
            _recentProjectsService.AddProject(project.ProjectPath, project.ProjectName);

            // Вызываем событие для открытия проекта
            ProjectOpened?.Invoke(this, new ProjectOpenedEventArgs(project.ProjectPath));
        }

        private bool CanOpenProject(ProjectInfo? project) => project != null;

        private void RemoveProject(ProjectInfo? project)
        {
            if (project != null)
            {
                _recentProjectsService.RemoveProject(project.ProjectPath);
            }
        }

        private bool CanRemoveProject(ProjectInfo? project) => project != null;

        private void ClearAll(object? p)
        {
            _recentProjectsService.ClearAll();
        }

        private bool CanClearAll() => RecentProjects.Count > 0;

        // Метод для добавления проекта извне (например, при открытии через File->Open)
        public void AddRecentProject(string path, string name)
        {
            _recentProjectsService.AddProject(path, name);
            IsVisible = true;
        }
    }

    // EventArgs для события открытия проекта
    public class ProjectOpenedEventArgs : EventArgs
    {
        public string ProjectPath { get; }

        public ProjectOpenedEventArgs(string projectPath)
        {
            ProjectPath = projectPath;
        }
    }
}
