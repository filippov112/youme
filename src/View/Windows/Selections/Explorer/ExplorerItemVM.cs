using Core.Explorer.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using View.Other;
using View.Windows.Selections.Models;

namespace View.Windows.Selections.Explorer
{
    public class ExplorerItemVM : ViewModel
    {
        private ICommand ChangeSelectingStateCommand {  get; set; }
        public ExplorerItemVM(ExplorerItemVM? parent, ProjectUnit elementDto, ICommand changeSelectingStateCommand)
        {
            Parent = parent;
            Name = elementDto.Name;
            FullPath = elementDto.Path;
            Type = elementDto.IsDirectory ? ItemType.Folder : ItemType.File;
            Children = [];
            foreach (var item in elementDto.Children)
            {
                Children.Add(new ExplorerItemVM(this, item, changeSelectingStateCommand));
            }
            ChangeSelectingStateCommand = changeSelectingStateCommand;
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

        public bool IsSelected // Элемент участвует в выборке (выбор через дерево проекта)
        {
            get => _isSelected;
            set
            {
                var oldValue = _isSelected;
                _isSelected = value;
                if (Type == ItemType.File && ((!oldValue && value) || (oldValue && !value)))
                    ChangeSelectingStateCommand.Execute(this);
                
                foreach (var item in Children)
                    item.IsSelected = value;
                OnPropertyChanged();
            }
        }
        public void Select(bool val = true) // Элемент участвует в выборке (выбор через список файлов)
        {
            _isSelected = val;
            foreach (var item in Children)
                item.Select(val);
            OnPropertyChanged(nameof(IsSelected));
        }
    }
    public enum ItemType
    {
        Folder,
        File
    }
}
