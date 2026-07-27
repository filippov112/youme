using Core.Explorer.Models;
using System.Collections.ObjectModel;
using View.Other;
using View.Windows.Selections.Models;

namespace View.Windows.Selections.Explorer
{
    public class ExplorerItemVM : ViewModel
    {
        private ObservableCollection<FileVM> _files;
        public ExplorerItemVM(ExplorerItemVM? parent, ProjectUnit elementDto, ObservableCollection<FileVM> files)
        {
            Parent = parent;
            Name = elementDto.Name;
            FullPath = elementDto.Path;
            Type = elementDto.IsDirectory ? ItemType.Folder : ItemType.File;
            Children = [];
            foreach (var item in elementDto.Children)
            {
                Children.Add(new ExplorerItemVM(this, item, files));
            }

            _files = files;
        }

        public void GetSelectedFiles(HashSet<string> container)
        {
            if (Type == ItemType.File && IsSelected)
                container.Add(FullPath);
            foreach (var item in Children)
                item.GetSelectedFiles(container);
        }
        public void GetFiles(List<ExplorerItemVM> container)
        {
            if (Type == ItemType.File)
                container.Add(this);
            foreach (var item in Children)
                item.GetFiles(container);
        }
        public void Expand()
        {
            IsExpanded = true;
            Parent?.Expand();
        }


        private bool _isExpanded;
        private bool _isSelected;

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

        public bool IsSelected // Элемент участвует в выборке
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                if (Type == ItemType.File && !_isSelected && value) // Add
                {
                    _files.Add(new() { Path = FullPath, Item = this });
                }
                else if (Type == ItemType.File && _isSelected && !value) // Delete
                {
                    var item = _files.FirstOrDefault(x => x.Path == FullPath);
                    if (item is not null)
                    {
                        _files.Remove(item);
                    }
                }
                foreach (var item in Children)
                    item.IsSelected = value;
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
