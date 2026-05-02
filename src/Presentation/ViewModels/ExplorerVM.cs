using Application.Interfaces;
using Application.Models;
using Presentation.Interfaces;
using Presentation.Other;
using System.Collections.ObjectModel;
using System.IO;

namespace Presentation.ViewModels
{
    public class ExplorerVM: ViewModel
    {
        private readonly IConfigService _config;
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
        private readonly List<string> _expandedPaths = [];
        private readonly List<string> _selectedPaths = [];

        public event Action<string, string> ObjectRenamed;
        
        public ExplorerVM(IConfigService config, IDialogService dialogService)
        {
            ExplorerContextMenu = new ExplorerMenuVM(this, dialogService);
            _config = config;
        }
        public ExplorerMenuVM ExplorerContextMenu { get; set; }
        
        
        public DataObject Move(ExplorerItemVM item)
        {
            DataObject data = new("ExplorerItemVM", item);
            data.SetData(DataFormats.Text, Path.GetRelativePath(_config.RootDirectory, item.FullPath));
            return data;
        }
        public void Move(string from, string to)
        {
            ObjectRenamed?.Invoke(from, to);
        }

        private void SaveTreeState()
        {
            ClearTreeState();
            foreach (var item in Items)
            {
                item.GetExpandedDirectories(_expandedPaths);
                item.GetSelectedFiles(_selectedPaths);
            }
        }

        private void RestoreTreeState()
        {
            foreach (var item in Items)
            {
                item.RestoreExpandedState(_expandedPaths);
                item.RestoreSelectedState(_selectedPaths);
            }
            ClearTreeState();
        }
        private void ClearTreeState()
        {
            _expandedPaths.Clear();
            _selectedPaths.Clear();
        }

        /// <summary>
        /// Загрузка структуры проекта из файловой системы
        /// </summary>
        /// <param name="rootPath"></param>
        public void LoadProject(ProjectUnit? rootElement)
        {
            SaveTreeState();
            Items.Clear();
            if (rootElement == null)
            {
                ClearTreeState();
                return;
            }
            var rootItem = new ExplorerItemVM(null, rootElement);
            Items.Add(rootItem);
            RestoreTreeState();
            OnPropertyChanged();
        }
    }
}
