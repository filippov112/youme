using Pdp.App.Models;
using Pdp.UI.Other;
using System.Collections.ObjectModel;

namespace Pdp.UI.ViewModels
{
    public class ExplorerItemVM : ViewModel
    {
        public ExplorerItemVM(ExplorerItemVM? parent, ProjectUnit elementDto)
        {
            Parent = parent;
            Name = elementDto.Name;
            FullPath = elementDto.Path;
            Type = elementDto.IsDirectory ? ItemType.Folder : ItemType.File;
            Children = [];
            foreach (var item in elementDto.Children)
            {
                Children.Add(new ExplorerItemVM(this, item));
            }
        }

        public void GetSelectedFiles(HashSet<string> container)
        {
            if (Type == ItemType.File && IsSelected)
                container.Add(FullPath);
            foreach (var item in Children)
                item.GetSelectedFiles(container);
        }
        public void GetExpandedDirectories(HashSet<string> container)
        {
            if (IsExpanded)
                container.Add(FullPath);
            foreach (var item in Children)
                item.GetExpandedDirectories(container);
        }
        public void RestoreSelectedState(HashSet<string> paths)
        {
            if (paths.Contains(FullPath))
                IsSelected = true;
            foreach (var item in Children)
                item.RestoreSelectedState(paths);
        }
        public void RestoreExpandedState(HashSet<string> paths)
        {
            if (paths.Contains(FullPath))
                IsExpanded = true;
            foreach (var item in Children)
                item.RestoreExpandedState(paths);
        }

        private bool _isExpanded;
        private bool _isSelected;
        private bool _isFocused;

        public ItemType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public ExplorerItemVM? Parent { get; set; } = null;
        public ObservableCollection<ExplorerItemVM> Children { get; set; } = [];

        public bool IsExpanded // Каталог раскрыт
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }
        public bool IsSelected // Элемент выбран для парсинга
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                foreach (var item in Children)
                    item.IsSelected = value;
                OnPropertyChanged();
            }
        }
        public bool IsFocused // Элемент отображается в редакторе
        {
            get => _isFocused;
            set
            {
                _isFocused = value;
                OnPropertyChanged();
            }
        }
    }
    public enum ItemType
    {
        Folder,
        File
    }
}
