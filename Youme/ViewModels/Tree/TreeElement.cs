using System.Collections.ObjectModel;

namespace Youme.ViewModels.Tree
{
    // Базовый класс для элементов дерева
    public class TreeElement : ViewModel
    {
        private bool _isExpanded;
        private bool _isSelected;
        private bool _isFocused;

        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public ItemType Type { get; set; }
        public ObservableCollection<TreeElement> Children { get; set; } = [];

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
                foreach(var item in Children)
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
