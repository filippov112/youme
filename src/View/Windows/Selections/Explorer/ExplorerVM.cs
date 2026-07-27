using Core.Explorer.Models;
using System.Collections.ObjectModel;
using View.Other;
using View.Windows.Selections.Models;

namespace View.Windows.Selections.Explorer
{
    public class ExplorerVM : ViewModel
    {
        private ObservableCollection<ExplorerItemVM> _items = [];
        public ObservableCollection<ExplorerItemVM> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Загрузка структуры проекта из файловой системы
        /// </summary>
        /// <param name="rootPath"></param>
        public void LoadProject(ProjectUnit? rootElement, ObservableCollection<FileVM> files)
        {
            Items.Clear();
            if (rootElement == null)
                return;
            var rootItem = new ExplorerItemVM(null, rootElement, files);
            Items.Add(rootItem);
            OnPropertyChanged();
        }
    }
}
