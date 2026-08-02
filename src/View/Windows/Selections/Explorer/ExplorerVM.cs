using Core.Explorer.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using View.Other;

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
        public void LoadProject(ProjectUnit? rootElement, ICommand changeSelectingStateCommand)
        {
            Items.Clear();
            if (rootElement == null)
                return;
            var rootItem = new ExplorerItemVM(null, rootElement, changeSelectingStateCommand);
            Items.Add(rootItem);
            IsEnabled = true;
            OnPropertyChanged();
        }

        private bool _isEnabled = false;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public void Clear()
        {
            Items.Clear();
            IsEnabled = false;
        }
    }
}
