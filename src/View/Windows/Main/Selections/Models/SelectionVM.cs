using System.Collections.ObjectModel;
using System.Windows.Input;
using View.Other;
using View.Windows.Main.Explorer;

namespace View.Windows.Main.Selections.Models
{
    public class SelectionVM : ViewModel
    {
        public ICommand SelectCommand { get; set; }

        public string Name { get; set; } = string.Empty;

        private List<string> _files;
        private ObservableCollection<ExplorerItemVM> _items;

        public SelectionVM(string name, List<string> files, ObservableCollection<ExplorerItemVM> items)
        {
            Name = name;
            _files = files;
            _items = items;

            SelectCommand = new RelayCommand(SelectItems);
        }

        private void SelectItems(object? sender)
        {
            foreach (var file in _files)
            {
                foreach (var item in _items.Where(x => x.FullPath == file))
                {
                    item.IsSelected = true;
                    item.Expand();
                }
            }
        }
    }
}
