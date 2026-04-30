using Application.Services;
using Presentation.Other;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;


namespace Presentation.Elements.Explorer.Components
{
    public class ExplorerMenuVM : ViewModel
    {
        private readonly ExplorerTreeVM _project;
        private readonly Dispatcher _uiDispatcher;
        private readonly IDialogService _dialogService;

        public ExplorerMenuVM(ExplorerTreeVM project, Dispatcher uiDispatcher, IDialogService dialogService)
        {
            _project = project;
            _uiDispatcher = uiDispatcher;
            _dialogService = dialogService;

            CreateFileCommand = new RelayCommand<ExplorerElementVM>(CreateFile);
            CreateFolderCommand = new RelayCommand<ExplorerElementVM>(CreateFolder);
            DeleteCommand = new RelayCommand<ExplorerElementVM>(DeleteItem);
            ExcludeCommand = new RelayCommand<ExplorerElementVM>(ExcludeItem);
            RenameCommand = new RelayCommand<ExplorerElementVM>(RenameItem);
        }

        // Команды
        public ICommand CreateFileCommand { get; }
        public ICommand CreateFolderCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ExcludeCommand { get; }
        public ICommand RenameCommand { get; }

        private void CreateFile(ExplorerElementVM? selectedItem)
        {
            // Определяем родительский каталог, в котором нужно создать файл
            ExplorerElementVM? targetParent = selectedItem switch
            {
                null => _project?.Items?.FirstOrDefault(i => i.Type == ItemType.Folder), // Корень проекта
                { Type: ItemType.Folder } => selectedItem,                             // Выбранная папка
                { Type: ItemType.File, Parent: { } parent } => parent,                 // Родитель файла
                _ => null
            };

            if (targetParent?.Type != ItemType.Folder)
            {
                _dialogService.ShowWarning("Не удалось определить каталог для создания файла.");
                return;
            }

            var fileName = Microsoft.VisualBasic.Interaction.InputBox("Введите имя файла:", "Создать файл", "newfile.txt");
            if (string.IsNullOrWhiteSpace(fileName)) return;

            var fullPath = Path.Combine(targetParent.FullPath, fileName);
            if (File.Exists(fullPath) || Directory.Exists(fullPath))
            {
                _dialogService.ShowWarning("Файл или каталог с таким именем уже существует.");
                return;
            }

            try
            {
                File.WriteAllText(fullPath, string.Empty);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка при создании файла: {ex.Message}");
            }
        }

        private void CreateFolder(ExplorerElementVM? selectedItem)
        {
            // Определяем родительский каталог, в котором нужно создать папку
            ExplorerElementVM? targetParent = selectedItem switch
            {
                null => _project?.Items?.FirstOrDefault(i => i.Type == ItemType.Folder), // Корень проекта
                { Type: ItemType.Folder } => selectedItem,                             // Выбранная папка
                { Type: ItemType.File, Parent: { } parent } => parent,                 // Родитель файла
                _ => null
            };

            if (targetParent?.Type != ItemType.Folder)
            {
                _dialogService.ShowWarning("Не удалось определить каталог для создания папки.");
                return;
            }

            var folderName = Microsoft.VisualBasic.Interaction.InputBox("Введите имя каталога:", "Создать каталог", "NewFolder");
            if (string.IsNullOrWhiteSpace(folderName)) return;

            var fullPath = Path.Combine(targetParent.FullPath, folderName);
            if (File.Exists(fullPath) || Directory.Exists(fullPath))
            {
                _dialogService.ShowWarning("Файл или каталог с таким именем уже существует.");
                return;
            }

            try
            {
                Directory.CreateDirectory(fullPath);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка при создании каталога: {ex.Message}");
            }
        }

        private void DeleteItem(ExplorerElementVM? item)
        {
            if (item == null) return;

            var confirmMessage = item.Type == ItemType.Folder
                ? $"Удалить каталог '{item.Name}' и всё его содержимое?"
                : $"Удалить файл '{item.Name}'?";
            if (!_dialogService.ShowYesNoDialog(confirmMessage, "Подтверждение удаления")) 
                return;

            try
            {
                if (item.Type == ItemType.Folder)
                {
                    Directory.Delete(item.FullPath, true);
                }
                else
                {
                    File.Delete(item.FullPath);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка при удалении: {ex.Message}");
            }
        }

        private void ExcludeItem(ExplorerElementVM? item)
        {
            if (item == null) return;

            // Просто удаляем из дерева и из AllItems, не из файловой системы
            // Нужно найти родителя и удалить из его Children
            if (item.Parent != null)
            {
                item.Parent.Children.Remove(item);
                _project?.AllItems.Remove(item);
            }
            else if (_project?.Items != null)
            {
                // Если это корневой элемент (должен быть один)
                _project.Items.Remove(item);
                _project.AllItems.Remove(item);
            }
        }

        private void RenameItem(ExplorerElementVM? item)
        {
            if (item == null) return;

            var currentName = item.Name;
            var newName = Microsoft.VisualBasic.Interaction.InputBox("Введите новое имя:", "Переименовать", currentName);
            if (string.IsNullOrWhiteSpace(newName) || newName == currentName) return;

            var newFullPath = Path.Combine(Path.GetDirectoryName(item.FullPath) ?? ".", newName);
            if (File.Exists(newFullPath) || Directory.Exists(newFullPath))
            {
                _dialogService.ShowError("Файл или каталог с таким именем уже существует.");
                return;
            }

            try
            {
                if (item.Type == ItemType.Folder)
                {
                    Directory.Move(item.FullPath, newFullPath);
                }
                else
                {
                    File.Move(item.FullPath, newFullPath);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Ошибка при переименовании: {ex.Message}");
            }
        }
    }
}