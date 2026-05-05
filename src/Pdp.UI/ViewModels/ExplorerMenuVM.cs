using Pdp.App.Interfaces;
using Pdp.UI.Interfaces;
using Pdp.UI.Other;
using System.IO;
using System.Windows.Input;

namespace Pdp.UI.ViewModels
{
    public class ExplorerMenuVM : ViewModel
    {
        private readonly ExplorerVM _explorer;
        private readonly IDialogService _dialogService;
        private readonly IFileSystemManager _fsm;

        public ExplorerMenuVM(ExplorerVM explorer, IDialogService dialogService, IFileSystemManager fsm)
        {
            _fsm = fsm;
            _explorer = explorer;
            _dialogService = dialogService;

            CreateFileCommand = new RelayCommand<ExplorerItemVM>(CreateFile);
            CreateFolderCommand = new RelayCommand<ExplorerItemVM>(CreateFolder);
            DeleteCommand = new RelayCommand<ExplorerItemVM>(DeleteItem);
            ExcludeCommand = new RelayCommand<ExplorerItemVM>(ExcludeItem);
            RenameCommand = new RelayCommand<ExplorerItemVM>(RenameItem);
        }

        // Команды
        public ICommand CreateFileCommand { get; }
        public ICommand CreateFolderCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ExcludeCommand { get; }
        public ICommand RenameCommand { get; }

        private void CreateFile(ExplorerItemVM? selectedItem)
        {
            ExplorerItemVM? targetParent = selectedItem switch
            {
                null => _explorer?.Items?.FirstOrDefault(i => i.Type == ItemType.Folder), // Корень проекта
                { Type: ItemType.Folder } => selectedItem,                             // Выбранная папка
                { Type: ItemType.File, Parent: { } parent } => parent,                 // Родитель файла
                _ => null
            };
            if (targetParent?.Type != ItemType.Folder)
            {
                _dialogService.ShowWarning("Не удалось определить каталог для создания файла.");
                return;
            }

            var fileName = _dialogService.ShowInputTextDialog("Введите имя файла:", "Создать файл", "newfile.txt");
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var fullPath = Path.Combine(targetParent.FullPath, fileName);
            Task.Run(() => _fsm.WriteFileAsync(fullPath, string.Empty));
        }

        private void CreateFolder(ExplorerItemVM? selectedItem)
        {
            ExplorerItemVM? targetParent = selectedItem switch
            {
                null => _explorer?.Items?.FirstOrDefault(i => i.Type == ItemType.Folder), // Корень проекта
                { Type: ItemType.Folder } => selectedItem,                             // Выбранная папка
                { Type: ItemType.File, Parent: { } parent } => parent,                 // Родитель файла
                _ => null
            };
            if (targetParent?.Type != ItemType.Folder)
            {
                _dialogService.ShowWarning("Не удалось определить каталог для создания папки.");
                return;
            }
            var folderName = _dialogService.ShowInputTextDialog("Введите имя каталога:", "Создать каталог", "NewFolder");
            if (string.IsNullOrWhiteSpace(folderName))
                return;

            var fullPath = Path.Combine(targetParent.FullPath, folderName);
            Task.Run(() => _fsm.CreateDirectoryAsync(fullPath));
        }

        private void DeleteItem(ExplorerItemVM? item)
        {
            if (item == null)
                return;

            var confirmMessage = item.Type == ItemType.Folder
                ? $"Удалить каталог '{item.Name}' и всё его содержимое?"
                : $"Удалить файл '{item.Name}'?";
            if (!_dialogService.ShowYesNoDialog(confirmMessage, "Подтверждение удаления"))
                return;
            Task.Run(() => _fsm.DeleteAsync(item.FullPath));
        }

        private void ExcludeItem(ExplorerItemVM? item)
        {
            if (item == null)
                return;
            //ObjectExcluded?.Invoke(item.FullPath);
        }

        private void RenameItem(ExplorerItemVM? item)
        {
            if (item == null) return;

            var currentName = item.Name;
            var newName = _dialogService.ShowInputTextDialog("Введите новое имя:", "Переименовать", currentName);
            if (string.IsNullOrWhiteSpace(newName) || newName == currentName)
                return;

            var newFullPath = Path.Combine(Path.GetDirectoryName(item.FullPath) ?? ".", newName);
            Task.Run(() => _fsm.ChangePathAsync(item.FullPath, newFullPath));
        }
    }
}
