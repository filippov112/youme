using System.Collections.ObjectModel;
using System.Windows;
using Youme.Other;

namespace Youme.Elements.Tree
{
    // Базовый класс для элементов дерева
    public class TreeElement : ViewModel
    {
        private bool _isExpanded;
        private bool _isSelected;
        private bool _isFocused;
        private bool _isEnabled = true;

        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public TreeElement? Parent { get; set; } = null;

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
                foreach(var item in Children.Where(item => item.IsEnabled))
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
        public bool IsEnabled  // Элемент соответствует фильтру поиска
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
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
