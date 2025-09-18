using System.Collections.ObjectModel;
using System.IO;

namespace Youme.ViewModels.Tree
{
    public class TreeModel : ViewModel
    {
        public ObservableCollection<TreeElement> AllItems { get; set; } = [];
        private ObservableCollection<TreeElement> _items = [];
        private TreeElement? _selectedItem;

        public ObservableCollection<TreeElement> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public TreeElement? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                OnItemSelected?.Invoke(value);
            }
        }

        public Action<TreeElement?>? OnItemSelected { get; set; }

        public TreeModel()
        {
            Items = new ObservableCollection<TreeElement>();
        }

        // Загрузка структуры проекта из файловой системы
        public void LoadProject(string rootPath)
        {
            Items.Clear();
            var rootItem = CreateTreeItem(new DirectoryInfo(rootPath));
            Items.Add(rootItem);
        }

        /// <summary>
        /// Рекурсивный перебор проекта
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private TreeElement CreateTreeItem(FileSystemInfo info)
        {
            var item = new TreeElement
            {
                Name = info.Name,
                FullPath = info.FullName,
                Type = info is DirectoryInfo ? ItemType.Folder : ItemType.File,
                Children = new ObservableCollection<TreeElement>()
            };
            AllItems.Add(item);

            if (info is DirectoryInfo directory)
            {
                try
                {
                    foreach (var dir in directory.GetDirectories())
                    {
                        if ((dir.Attributes & FileAttributes.Hidden) == 0)
                        {
                            var child = CreateTreeItem(dir);
                            item.Children.Add(child);
                        }
                    }

                    foreach (var file in directory.GetFiles())
                    {
                        if ((file.Attributes & FileAttributes.Hidden) == 0)
                        {
                            var child = CreateTreeItem(file);
                            item.Children.Add(child);
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {}
            }

            return item;
        }
    }
}
