using System.Collections.ObjectModel;
using System.IO;
using Youme.Other;
using Youme.Services;

namespace Youme.Elements.Tree
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

        public TreeModel()
        {
            Items = new ObservableCollection<TreeElement>();
        }

        // Загрузка структуры проекта из файловой системы
        public void LoadProject(string rootPath)
        {
            AllItems.Clear();
            Items.Clear();
            _serviceDir = Path.Combine(Program.Storage.ProjectFolder, StorageService.LocalConfigFolder);
            var rootItem = CreateTreeItem(null, new DirectoryInfo(rootPath));
            Items.Add(rootItem);
            OnPropertyChanged();
        }

        private string _serviceDir;

        /// <summary>
        /// Рекурсивный перебор проекта
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private TreeElement CreateTreeItem(TreeElement? parent, FileSystemInfo info)
        {
            var item = new TreeElement
            {
                Name = info.Name,
                FullPath = info.FullName,
                Type = info is DirectoryInfo ? ItemType.Folder : ItemType.File,
                Text = info is DirectoryInfo ? string.Empty : ContentBuilder.ShouldInclude(info.FullName) ? ContentBuilder.ParseFile(info.FullName) : string.Empty,
                Children = [],
                Parent = parent
            };
            AllItems.Add(item);

            if (info is DirectoryInfo directory)
            {
                try
                {
                    foreach (var dir in directory.GetDirectories())
                    {
                        if (Path.Combine(dir.FullName) == _serviceDir)
                            continue;
                        if ((dir.Attributes & FileAttributes.Hidden) == 0)
                        {
                            var child = CreateTreeItem(item, dir);
                            item.Children.Add(child);
                        }
                    }

                    foreach (var file in directory.GetFiles())
                    {
                        if ((file.Attributes & FileAttributes.Hidden) == 0)
                        {
                            var child = CreateTreeItem(item, file);
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
