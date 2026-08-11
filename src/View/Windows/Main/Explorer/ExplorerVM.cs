using Core.Explorer.Models;
using Core.Explorer.Services;
using Core.Settings.Services;
using System.Collections.ObjectModel;
using System.IO;
using View.Other;
using View.Services;

namespace View.Windows.Main.Explorer
{
    public class ExplorerVM : ViewModel
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
        private readonly HashSet<string> _expandedPaths = [];
        private readonly HashSet<string> _selectedPaths = [];
        private readonly IFileSystemService _fsm;
        public Action<string>? OpenFile;
        public ExplorerVM(IConfigService config, IDialogService dialogService, IFileSystemService fsm)
        {
            _fsm = fsm;
            ExplorerContextMenu = new ExplorerMenuVM(this, dialogService, fsm);
            _config = config;
        }
        public ExplorerMenuVM ExplorerContextMenu { get; set; }
        public DataObject MoveToQuery(ExplorerItemVM item)
        {
            DataObject data = new("ExplorerItemVM", item);
            data.SetData(DataFormats.Text, Path.GetRelativePath(_config.RootDirectory, item.FullPath));
            return data;
        }
        public void MoveObjectBetweenDirectories(string from, string to)
        {
            Task.Run(async () => await _fsm.ChangePathAsync(from, to));
        }

        private void SaveTreeState()
        {
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
        }
        public void ClearTreeState()
        {
            _expandedPaths.Clear();
            _selectedPaths.Clear();
        }

        /// <summary>
        /// Загрузка структуры проекта из файловой системы
        /// </summary>
        /// <param name="rootPath"></param>
        public void LoadProject(ProjectUnit? rootElement, bool fastReload = true)
        {
            ClearTreeState();
            if (fastReload)
                SaveTreeState();
            Items.Clear();
            if (rootElement == null)
                return;
            var rootItem = new ExplorerItemVM(null, rootElement);
            Items.Add(rootItem);
            if (fastReload)
                RestoreTreeState();
            ClearTreeState();
            OnPropertyChanged();
        }
    }
}
