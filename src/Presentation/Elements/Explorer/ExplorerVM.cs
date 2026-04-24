using Application.Services;
using Infrastructure.Services;
using Presentation.Elements.Explorer.Components;
using Presentation.Other;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Threading;

namespace Presentation.Elements.Explorer
{
    public class ExplorerVM : ViewModel
    {
        public ExplorerTreeVM? Project { get; set; } = null;
        public ExplorerSearchVM Search { get; set; } = new ExplorerSearchVM(checkText: (item) => item.Text(), displayText: (item) => item.FullPath);
        public ExplorerMenuVM ExplorerContextMenu { get; set; }
        private readonly IStorageService _storageService;
        public ExplorerVM(Dispatcher uiDispatcher, IDialogService dialogService, IContentBuilder contentBuilder, IStorageService storageService)
        {
            _storageService = storageService;
            Project = new ExplorerTreeVM(uiDispatcher, dialogService, contentBuilder, storageService);
            Search.Tree = Project;
            ExplorerContextMenu = new ExplorerMenuVM(Project, uiDispatcher, dialogService);
        }

        public DataObject Move(ExplorerElementVM _draggedItem)
        {
            // Формируем данные для перетаскивания
            DataObject data = new DataObject("ExplorerElementVM", _draggedItem);
            // Дополнительно можно передать текстовое представление
            data.SetData(DataFormats.Text, Path.GetRelativePath(_storageService.ProjectFolder, _draggedItem.FullPath));
            return data;
        }

        /// <summary>
        /// Массив файлов и каталогов для парсинга
        /// </summary>
        public ObservableCollection<ExplorerElementVM> AllItems
        {
            get
            {
                return Project?.AllItems ?? new ObservableCollection<ExplorerElementVM>();
            }
        }

        /// <summary>
        /// Перезагрузка дерева проекта
        /// </summary>
        public void Refresh()
        {
            Project?.Refresh();
        }
    }
}
