using Core.Explorer.Services;
using Core.PromptBuilder.Service;
using Core.Settings.Services;
using Core.Tools;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Windows.Input;
using View.Other;
using View.Services;
using View.Windows.Main.Editor;
using View.Windows.Main.Explorer;
using View.Windows.Main.RecentProjects;
using View.Windows.Settings;

namespace View.Windows.Main
{
    public class MainWindowVM : ViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IConfigService _configService;
        private readonly IFileSystemService _fileSystemService;
        private readonly IPromptService _promptService;
        private readonly ITokenCounterTool _tokenCounterTool;
        private readonly IBufferExchangeTool _bufferExchangeTool;
        private readonly ICatalogObserver _catalogObserver;


        private readonly ICatalogChangeEvent _catalogChangeEvent; // Событие изменения в каталоге


        public MainWindowVM(
            ExplorerVM explorer,
            EditorVM editor,
            RecentProjectsVM projects,
            IDialogService dialogService,
            IConfigService configService,
            IFileSystemService fileSystemService,
            IPromptService promptService,
            ITokenCounterTool tokenCounterService,
            IBufferExchangeTool bufferExchangeService,
            ICatalogChangeEvent catalogChangeEvent,
            ICatalogObserver catalogObserver
            )
        {
            EditorViewModel = editor;
            ExplorerViewModel = explorer;
            RecentProjectsViewModel = projects;

            _fileSystemService = fileSystemService;
            _catalogObserver = catalogObserver;
            _promptService = promptService;
            _dialogService = dialogService;
            _configService = configService;
            _bufferExchangeTool = bufferExchangeService;
            _tokenCounterTool = tokenCounterService;

            ExplorerViewModel.OpenFile = async path => { await OpenDocument(path); };

            _catalogChangeEvent = catalogChangeEvent;
            _catalogChangeEvent.CatalogChanged += OnCatalogChanged;

            RecentProjectsViewModel.ProjectOpened += OnProjectOpened;

            OpenProjectCommand = new RelayCommand(OpenProjectDialog);
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            BuildPromptCommand = new RelayCommand(BuildPrompt);
        }

        private void OnProjectOpened(object? sender, ProjectOpenedEventArgs e)
        {
            // Открываем проект
            OpenProject(e.ProjectPath);
        }

        private void OnCatalogChanged()
        {
            Task.Run(async () =>
            {
                var tree = await _fileSystemService.GetTreeAsync();
                App.Current.Dispatcher.Invoke(() => ExplorerViewModel.LoadProject(tree));
                OnPropertyChanged();
            });
        }

        #region Editor
        public EditorVM EditorViewModel { get; private set; }
        #endregion

        #region Query
        // Query
        private string _query = string.Empty;
        public string Query
        {
            get => _query;
            set
            {
                _query = value;
                OnPropertyChanged();
            }
        }

        // Tokens
        private string _tokens = string.Empty;
        public string Tokens
        {
            get => _tokens;
            set
            {
                _tokens = value;
                OnPropertyChanged();
            }
        }

        // Build
        public ICommand BuildPromptCommand { get; }
        private void BuildPrompt(object? _)
        {
            Task.Run(async () =>
            {
                var files = new HashSet<string>();
                if (ExplorerViewModel.Items.Count > 0)
                    ExplorerViewModel.Items[0].GetSelectedFiles(files);
                var filesList = files.Union(ExplorerViewModel.SelectedFiles).ToList();
                var text = await _promptService.GetPrompt(filesList, Query);
                App.Current.Dispatcher.Invoke(() =>
                {
                    EditorViewModel.Text = text;
                    EditorViewModel.Highlight = HighlightingManager.Instance.GetDefinition("markdown");
                    Tokens = _tokenCounterTool.CalcTokenCount(EditorViewModel.Text).ToString();
                    _bufferExchangeTool.Copy(EditorViewModel.Text);
                });
            });
        }
        #endregion

        #region Menu
        public RecentProjectsVM RecentProjectsViewModel { get; private set; }

        public ICommand OpenProjectCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        private void OpenProjectDialog(object? _)
        {
            string? rootPath = _dialogService.ShowOpenFolderDialog();
            if (rootPath == null)
                return;
            OpenProject(rootPath);
        }

        private void OpenProject(string rootPath)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(rootPath);

            Task.Run(async () =>
            {
                await _configService.SetRootDirectoryAsync(rootPath);
                var tree = await _fileSystemService.GetTreeAsync();
                App.Current.Dispatcher.Invoke(() =>
                {
                    ExplorerViewModel.ClearTreeState();
                    ExplorerViewModel.LoadProject(tree, false);
                    EditorViewModel.Text = string.Empty;
                    _catalogObserver.StartObserving();

                    // Добавляем в список последних
                    RecentProjectsViewModel.AddRecentProject(rootPath, name);
                });
            });
        }

        private void OpenSettings(object? e)
        {
            var window = new SettingsWindow(new SettingsWindowVM(_configService, _dialogService));
            window.ShowDialog();
        }
        #endregion

        #region Explorer
        public ExplorerVM ExplorerViewModel { get; private set; }
        public async Task OpenDocument(string path)
        {
            EditorViewModel.Text = await _fileSystemService.ReadFileAsync(path);
            EditorViewModel.SetHighlight(path);
        }
        #endregion
    }
}
