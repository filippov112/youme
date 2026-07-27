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

        private readonly ProjectUnit? tree;

        /// <summary>
        /// Выборки
        /// </summary>
        public ObservableCollection<SelectionVM> Selections { get; private set; } = [];
        /// <summary>
        /// Дерево проекта
        /// </summary>
        public ExplorerVM ExplorerViewModel { get; private set; }
        /// <summary>
        /// Файлы
        /// </summary>
        public ObservableCollection<FileVM> Files { get; private set; } = [];


        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand SelectSelectionCommand { get; private set; }
        public ICommand RenameSelectionCommand  { get; private set; }
        public ICommand DeleteSelectionCommand { get; private set; }
        public ICommand CreateSelectionCommand { get; private set; }

        public SelectionSettingsWindowVM(ISelectionService selectionService, IDialogService dialogService, IFileSystemService fileSystemService)
        {
            _selectionService = selectionService;
            _dialogService = dialogService;
            _fileSystemService = fileSystemService;
            ExplorerViewModel = new();

            SelectSelectionCommand = new RelayCommand<SelectionVM>(SelectSelection);
            CreateSelectionCommand = new RelayCommand(CreateSelection);
            RenameSelectionCommand = new RelayCommand(RenameSelection);
            DeleteSelectionCommand = new RelayCommand(DeleteSelection);
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);

            try
            {
                var selections = Task.Run(_selectionService.GetSelections).Result;
                tree = Task.Run(_fileSystemService.GetTreeAsync).Result;

                App.Current.Dispatcher.Invoke(() =>
                {
                    ExplorerViewModel.LoadProject(tree, Files);

                    var items = new List<ExplorerItemVM>();
                    if (ExplorerViewModel.Items.Count > 0)
                        ExplorerViewModel.Items[0].GetFiles(items);

                    foreach (var selection in selections)
                    {
                        List<FileVM> selectionItems = new();
                        foreach(var file in selection.Files)
                        {
                            bool exist = false;
                            foreach(var item in items)
                            {
                                if (item.FullPath == file)
                                {
                                    selectionItems.Add(new() { Path = file, Item = item });
                                    exist = true;
                                    break;
                                }
                            }
                            if (!exist)
                                selectionItems.Add(new() { Path = file });
                        }
                        SelectionVM selVM = new(selection.Name, new(selectionItems));
                        Selections.Add(selVM);
                    }
                });
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка загрузки выборок: {ex.Message}");
            }
        }

        #region Selections
        private void SelectSelection(SelectionVM? vM)
        {
            throw new NotImplementedException();
        }
        private void CreateSelection(object? sender)
        {
            throw new NotImplementedException();
        }
        private void RenameSelection(object? sender)
        {
            throw new NotImplementedException();
        }
        private void DeleteSelection(object? sender)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Saving
        public void Save(object? sender)
        {

        }
        public bool CanSave()
        {
            return true;
        }
        #endregion


        #region Closing
        private void Cancel(object? _)
        {
            if (OnCloced())
                CloseWindow();
        }
        public bool OnCloced()
        {
            if (HasChanges())
            {
                var result = _dialogService.ShowYesNoDialog("У вас есть несохраненные изменения. Вы уверены, что хотите закрыть окно?",
                                           "Подтверждение закрытия");

                if (!result)
                    return false;
            }
            return true;
        }
        private bool HasChanges()
        {
            // ...
            return false;
        }
        private void CloseWindow()
        {
            if (System.Windows.Application.Current.Windows.Count > 0)
            {
                foreach (var window in System.Windows.Application.Current.Windows)
                {
                    if (window is Window w && w.DataContext == this)
                    {
                        w.Close();
                        break;
                    }
                }
            }
        }
        #endregion
    }
}
