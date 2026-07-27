using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using View.Other;

namespace View.Windows.Selections.Models
{
    public class SelectionVM: ViewModel
    {
        private string _name = string.Empty;
        public ICommand SelectCommand { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FileVM> Files { get; private set; } = [];

        public SelectionVM(string name, ObservableCollection<FileVM> files)
        {
            Name = name;
            Files = files;

            SelectCommand = new RelayCommand(SelectItems);
        }

        private void SelectItems(object? sender)
        {
            foreach (var file in Files)
            {
                file.Item?.IsSelected = true;
                file.Item?.Expand();
            }
        }
    }
}
