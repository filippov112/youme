using Pdp.Domain.Models.LastProjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using Pdp.App.Interfaces;


namespace Pdp.App.Services
{
    public class RecentProjectsService : IRecentProjectsService
    {
        private readonly ObservableCollection<ProjectInfo> _recentProjects;
        private readonly string _storageFilePath;
        private int _maxCount = 10;

        public RecentProjectsService()
        {
            _recentProjects = new ObservableCollection<ProjectInfo>();
            _storageFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Schiza",
                "recent_projects.json"
            );

            // Подписываемся на изменения коллекции
            _recentProjects.CollectionChanged += OnCollectionChanged;

            LoadFromStorage();
        }

        public ObservableCollection<ProjectInfo> RecentProjects => _recentProjects;

        public int MaxCount
        {
            get => _maxCount;
            set
            {
                if (value > 0)
                {
                    _maxCount = value;
                    TrimCollection();
                }
            }
        }

        public void AddProject(string projectPath, string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectPath))
                return;

            // Проверяем, существует ли проект уже в списке
            var existing = _recentProjects.FirstOrDefault(p => p.ProjectPath == projectPath);

            if (existing != null)
            {
                // Обновляем время открытия и перемещаем в начало
                existing.LastOpened = DateTime.Now;
                existing.ProjectName = projectName;
                _recentProjects.Move(_recentProjects.IndexOf(existing), 0);
            }
            else
            {
                // Добавляем новый проект в начало
                _recentProjects.Insert(0, new ProjectInfo
                {
                    ProjectPath = projectPath,
                    ProjectName = projectName,
                    LastOpened = DateTime.Now
                });
            }

            TrimCollection();
            SaveToStorage();
        }

        public void RemoveProject(string projectPath)
        {
            var project = _recentProjects.FirstOrDefault(p => p.ProjectPath == projectPath);
            if (project != null)
            {
                _recentProjects.Remove(project);
                SaveToStorage();
            }
        }

        public void ClearAll()
        {
            _recentProjects.Clear();
            SaveToStorage();
        }

        public void LoadFromStorage()
        {
            try
            {
                if (File.Exists(_storageFilePath))
                {
                    var json = File.ReadAllText(_storageFilePath);
                    var projects = JsonSerializer.Deserialize<List<ProjectInfo>>(json);

                    if (projects != null)
                    {
                        _recentProjects.Clear();
                        foreach (var project in projects.OrderByDescending(p => p.LastOpened))
                        {
                            _recentProjects.Add(project);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                System.Diagnostics.Debug.WriteLine($"Error loading recent projects: {ex.Message}");
            }
        }

        public void SaveToStorage()
        {
            try
            {
                var directory = Path.GetDirectoryName(_storageFilePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var json = JsonSerializer.Serialize(_recentProjects, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_storageFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving recent projects: {ex.Message}");
            }
        }

        private void TrimCollection()
        {
            while (_recentProjects.Count > _maxCount)
            {
                _recentProjects.RemoveAt(_recentProjects.Count - 1);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Автоматическое сохранение при изменениях
            SaveToStorage();
        }
    }
}
