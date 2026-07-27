using Core.Explorer.Models;
using Core.Explorer.Services;
using Core.Selections.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using View.Other;
using View.Services;
using View.Windows.Selections.Explorer;
using View.Windows.Selections.Models;

namespace View.Windows.Selections
{
    public class SelectionSettingsWindowVM: ViewModel
    {
        private readonly ISelectionService _selectionService;
        private readonly IDialogService _dialogService;
        private readonly IFileSystemService _fileSystemService;

        
        /// <summary>
        /// Выборки
        /// </summary>
        public ObservableCollection<SelectionVM> Selections { get; private set; } = [];
        
        /// <summary>
        /// Дерево проекта
        /// </summary>
        public ExplorerVM ExplorerViewModel { get; private set; }
        private readonly ProjectUnit? tree;
        private List<ExplorerItemVM>? treeItems;
        /// <summary>
        /// Файлы
        /// </summary>
        public ObservableCollection<FileVM>? Files { get; private set; } = [];

        public SelectionSettingsWindowVM(ISelectionService selectionService, IDialogService dialogService, IFileSystemService fileSystemService)
        {
            _selectionService = selectionService;
            _dialogService = dialogService;
            _fileSystemService = fileSystemService;
            ExplorerViewModel = new();

            
            CreateSelectionCommand = new RelayCommand(CreateSelection);
            RenameSelectionCommand = new RelayCommand(RenameSelection, () => SelectedSelection is not null);
            DeleteSelectionCommand = new RelayCommand(DeleteSelection, () => SelectedSelection is not null);

            CreateFilePathCommand = new RelayCommand(CreateFilePath, () => SelectedSelection is not null);
            ChangeFilePathCommand = new RelayCommand(ChangeFilePath, () => SelectedSelection is not null && SelectedFilePath is not null);
            DeleteFilePathCommand = new RelayCommand(DeleteFilePath, () => SelectedSelection is not null && SelectedFilePath is not null);

            SaveCommand = new RelayCommand(Save, CanSave);
            ChangeSelectingStateCommand = new RelayCommand<ExplorerItemVM>(ChangeSelectingState);

            try
            {
                var selections = Task.Run(_selectionService.GetSelections).Result;
                tree = Task.Run(_fileSystemService.GetTreeAsync).Result;

                App.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var selection in selections)
                        Selections.Add(new SelectionVM(selection.Name, new(selection.Files.Select(x => new FileVM(x)))));
                });
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка загрузки выборок: {ex.Message}");
            }
        }

        #region Selections
        public ICommand RenameSelectionCommand { get; private set; }
        public ICommand DeleteSelectionCommand { get; private set; }
        public ICommand CreateSelectionCommand { get; private set; }

        private string _currentSelectionName;
        public string CurrentSelectionName 
        {
            get => _currentSelectionName;
            set
            {
                _currentSelectionName = value;
                OnPropertyChanged();
            }
        }
        private SelectionVM? _selectedSelection;
        public SelectionVM? SelectedSelection
        {
            get => _selectedSelection;
            set
            {
                _selectedSelection = value;
                OnPropertyChanged();
                LoadSelection(value);
            }
        }

        private void LoadSelection(SelectionVM? selection)
        {
            ExplorerViewModel.LoadProject(tree, ChangeSelectingStateCommand);

            treeItems = [];
            if (ExplorerViewModel.Items.Count > 0)
                ExplorerViewModel.Items[0].GetFiles(treeItems);

            if (selection is not null)
                foreach (var file in selection.Files)
                    file.FindItem(treeItems);

            CurrentSelectionName = selection?.Name ?? "";
            Files = selection?.Files;
            OnPropertyChanged(nameof(Files));
            SelectedFilePath = null;
        }

        private void CreateSelection(object? sender)
        {
            var newSelection = new SelectionVM("New Selection", []);
            Selections.Add(newSelection);
            selectionsChanged = true;
            SelectedSelection = newSelection;
        }
        private void RenameSelection(object? sender)
        {
            SelectedSelection!.Name = CurrentSelectionName;
            selectionsChanged = true;
            OnPropertyChanged(nameof(SelectedSelection.Name));
        }
        private void DeleteSelection(object? sender)
        {
            Selections.Remove(SelectedSelection!);
            selectionsChanged = true;
            SelectedSelection = null;
        }
        #endregion


        #region Files
        public ICommand SelectFilePathCommand { get; private set; }
        public ICommand ChangeFilePathCommand { get; private set; }
        public ICommand DeleteFilePathCommand { get; private set; }
        public ICommand CreateFilePathCommand { get; private set; }
        private string _currentFilePath;
        public string CurrentFilePath
        {
            get => _currentFilePath;
            set
            {
                _currentFilePath = value;
                OnPropertyChanged();
            }
        }
        private FileVM? _selectedFilePath;
        public FileVM? SelectedFilePath
        {
            get => _selectedFilePath;
            set
            {
                _selectedFilePath = value;
                CurrentFilePath = value?.Path ?? "";
                OnPropertyChanged();
            }
        }
        private void CreateFilePath(object? sender)
        {
            var newPath = new FileVM("");
            newPath.FindItem(treeItems!);
            Files!.Add(newPath);
            OnPropertyChanged(nameof(Files));
            selectionsChanged = true;
            SelectedFilePath = newPath;
        }
        private void ChangeFilePath(object? sender)
        {
            SelectedFilePath!.Path = CurrentFilePath;
            selectionsChanged = true;
            OnPropertyChanged(nameof(SelectedFilePath.Path));
        }
        private void DeleteFilePath(object? sender)
        {
            SelectedFilePath!.Item?.Select(false);
            SelectedFilePath!.Item?.Expand();
            Files!.Remove(SelectedFilePath!);
            selectionsChanged = true;
            SelectedFilePath = null;
        }
        #endregion


        #region Tree
        public ICommand ChangeSelectingStateCommand { get; set; }
        private void ChangeSelectingState(ExplorerItemVM? item)
        {
            if (item is null)
                return;
            if (item.IsSelected)
            {
                var newFile = new FileVM(item.FullPath) { Item = item };
                Files!.Add(newFile);
            }
            else
            {
                var deletedFile = Files!.FirstOrDefault(f => f.Item == item);
                if (deletedFile is null)
                    return;
                Files!.Remove(deletedFile);
            }
            selectionsChanged = true;
            OnPropertyChanged(nameof(Files));
        }
        #endregion

        #region Saving
        private bool _sCh = false;
        private bool selectionsChanged
        {
            get => _sCh;
            set
            {
                _sCh = value;
            }
        }
        public ICommand SaveCommand { get; private set; }
        public void Save(object? sender)
        {
            Task.Run(async () => { 
                await _selectionService.SaveSelections(Selections.Select(x => new Core.Selections.Models.Selection() { Name = x.Name, Files = [.. x.Files.Select(y => y.Path)] }));

                App.Current.Dispatcher.Invoke(() =>
                {
                    selectionsChanged = false;
                    OnPropertyChanged(nameof(selectionsChanged));
                });
            });
        }
        public bool CanSave() => selectionsChanged;
        #endregion


        #region Closing
        public bool ApproveClosing()
        {
            if (CanSave())
            {
                var result = _dialogService.ShowYesNoDialog("У вас есть несохраненные изменения. Вы уверены, что хотите закрыть окно?", "Подтверждение закрытия");
                if (!result)
                    return false;
            }
            return true;
        }
        #endregion
    }
}
