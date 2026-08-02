using System.Windows.Media;
using View.Other;
using View.Windows.Selections.Explorer;

namespace View.Windows.Selections.Models
{
    public class FileVM(string path) : ViewModel
    {
        private List<ExplorerItemVM>? _items;
        private string _path = path;
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged();
                CheckItem();
            }
        }

        public void FindItem(List<ExplorerItemVM> items)
        {
            _items = items;
            CheckItem();
        }

        private void CheckItem()
        {
            if (_items is null)
                return;
            Item = _items.FirstOrDefault(x => x.FullPath == _path);
            Item?.Select();
            Item?.Expand();
            OnPropertyChanged(nameof(IsExist));
        }
        public ExplorerItemVM? Item { get; set; }
        public SolidColorBrush IsExist => Item is not null ? new(Colors.LightGreen) : new(Colors.LightPink);
    }
}
