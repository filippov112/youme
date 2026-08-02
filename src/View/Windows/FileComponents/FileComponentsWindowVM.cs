using Core.Explorer.Services;
using Core.Layouts.DTO;
using Core.Layouts.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using View.Other;
using View.Services;
using View.Windows.FileComponents.Models;

namespace View.Windows.FileComponents
{
    public class FileComponentsWindowVM : ViewModel
    {
        private IFileComponentService _fileComponentService;
        private IDialogService _dialogService;
        private readonly IFileSystemService _fileSystemService;

        public FileComponentsWindowVM(IFileComponentService fileComponentService, IDialogService dialogService, IFileSystemService fileSystemService)
        {
            _fileComponentService = fileComponentService;
            _dialogService = dialogService;
            _fileSystemService = fileSystemService;

            CreateComponentCommand = new RelayCommand(CreateComponent, () => Components.FirstOrDefault(c => c.Path == CurrentPath) is null && !string.IsNullOrEmpty(CurrentPath));
            UpdateComponentCommand = new RelayCommand(UpdateComponent, () => SelectedComponent is not null);
            DeleteComponentCommand = new RelayCommand(DeleteComponent, () => SelectedComponent is not null);

            SearchFilesCommand = new RelayCommand((object? _) => { LoadFiles(SearchPattern); });

            SaveCommand = new RelayCommand(Save, CanSave);

            LoadComponents();
            LoadFiles();
        }


        #region Components
        private void LoadComponents()
        {
            Task.Run(async () =>
            {
                try
                {
                    var components = await _fileComponentService.GetAllComponents();

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var c in components)
                            Components.Add(new FileComponentVM(c));
                    });
                }
                catch (Exception ex)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        _dialogService.ShowError($"Ошибка загрузки компонентов: {ex.Message}");
                    });
                }
            });
        }
        /// <summary>
        /// Компоненты
        /// </summary>
        public ObservableCollection<FileComponentVM> Components { get; private set; } = [];

        public FileComponentVM? SelectedComponent
        {
            get; set
            {
                field = value;
                OnPropertyChanged();
                LoadComponent(value);
            }
        }
        private void LoadComponent(FileComponentVM? component)
        {
            CurrentPath = component?.Path ?? "";
            CurrentKey = component?.Key ?? "";
            CurrentName = component?.Name ?? "";
        }

        public string CurrentName { get; set { field = value; OnPropertyChanged(); } } = string.Empty;
        public string CurrentPath { get; set { field = value; OnPropertyChanged(); } } = string.Empty;
        public string CurrentKey { get; set { field = value; OnPropertyChanged(); } } = string.Empty;


        public ICommand UpdateComponentCommand { get; private set; }
        public ICommand DeleteComponentCommand { get; private set; }
        public ICommand CreateComponentCommand { get; private set; }
        private void CreateComponent(object? sender)
        {
            FileComponentDto dto = new() { Name = CurrentName, Key = CurrentKey, Path = CurrentPath };
            var component = new FileComponentVM(dto);
            Components.Add(component);
            componentsChanged = true;
            SelectedComponent = component;
        }
        private void UpdateComponent(object? sender)
        {
            SelectedComponent!.Name = CurrentName;
            SelectedComponent!.Key = CurrentKey;
            componentsChanged = true;
        }
        private void DeleteComponent(object? sender)
        {
            Components.Remove(SelectedComponent!);
            componentsChanged = true;
            SelectedComponent = null;
        }
        #endregion

        #region Files
        /// <summary>
        /// Файлы
        /// </summary>
        public ObservableCollection<string> Files { get; private set { field = value; OnPropertyChanged(); } } = [];

        public string? SelectedFilePath
        {
            get; set
            {
                field = value;
                SelectedComponent = null;
                CurrentPath = value ?? string.Empty;
                OnPropertyChanged();
            }
        }
        public string SearchPattern { get; set { field = value; OnPropertyChanged(); } } = string.Empty;
        public ICommand SearchFilesCommand { get; private set; }
        private void LoadFiles(string? searchPattern = null)
        {
            Task.Run(async () =>
            {
                try
                {
                    List<string> files = await _fileSystemService.GetFilesAsync();
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Files.Clear();
                        foreach (var file in files)
                            if (searchPattern is null || file.Contains(searchPattern))
                                Files.Add(file);
                    });
                }
                catch (Exception ex)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        _dialogService.ShowError($"Ошибка загрузки файлов: {ex.Message}");
                    });
                }
            });
        }
        #endregion


        #region Saving
        private bool _sCh = false;
        private bool componentsChanged
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
            Task.Run(async () =>
            {
                await _fileComponentService.SaveAllComponents(Components.Select(x => x.DTO).ToList());
            });
            componentsChanged = false;
        }
        public bool CanSave() => componentsChanged;
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
