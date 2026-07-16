using ICSharpCode.AvalonEdit.Highlighting;
using Pdp.App.Interfaces;
using Pdp.UI.Interfaces;
using Pdp.UI.Other;
using Pdp.UI.Windows;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Pdp.UI.ViewModels
{
    public class MainWindowVM : ViewModel
    {
        private readonly IDialogService _dialogs;
        private readonly IConfigService _cs;
        private readonly IFileSystemManager _fsm;
        private readonly IPromptBuilder _promptBuilder;
        private readonly ITokenCounter _counter;
        private readonly IBufferExchange _buffer;
        private readonly ICatalogChangedHandler _catalogChanged;
        private readonly IHighlightSelector _highlightSelector;
        private readonly ICatalogObserver _observer;
        private readonly IRecentProjectsService _recentProjectsService;

        private readonly RecentProjectsVM _recentProjectsViewModel;
        public RecentProjectsVM RecentProjects => _recentProjectsViewModel;

        public MainWindowVM(
            ExplorerVM explorer,
            IRecentProjectsService recentProjectsService,
            IDialogService dialogs,
            IConfigService cs,
            IFileSystemManager fsm,
            IPromptBuilder promptBuilder,
            ITokenCounter counter,
            IBufferExchange buffer,
            ICatalogChangedHandler catalogChanged,
            IHighlightSelector highlight,
            ICatalogObserver observer
            )
        {
            _recentProjectsService = recentProjectsService;
            _fsm = fsm;
            _observer = observer;
            _highlightSelector = highlight;
            _buffer = buffer;
            _counter = counter;
            _promptBuilder = promptBuilder;
            Explorer = explorer;
            Explorer.OpenFile = async path => { await OpenDocument(path); };
            _catalogChanged = catalogChanged;
            _catalogChanged.CatalogChanged += OnCatalogChanged;
            _dialogs = dialogs;
            _cs = cs;

            _recentProjectsViewModel = new(recentProjectsService);
            // Подписываемся на событие открытия проекта
            _recentProjectsViewModel.ProjectOpened += OnProjectOpened;


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
                var tree = await _fsm.GetTreeAsync();
                App.Current.Dispatcher.Invoke(() => Explorer.LoadProject(tree));
                OnPropertyChanged();
            });
        }

        #region Editor
        // Text
        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        // Highlight
        private IHighlightingDefinition _highlight = HighlightingManager.Instance.GetDefinition("markdown");
        public IHighlightingDefinition Highlight
        {
            get => _highlight;
            set
            {
                _highlight = value;
                OnPropertyChanged();
            }
        }
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
                if (Explorer.Items.Count > 0)
                    Explorer.Items[0].GetSelectedFiles(files);
                var filesList = files.Union(Explorer.SelectedFiles).ToList();
                var text = await _promptBuilder.GetPrompt(filesList, Query);
                App.Current.Dispatcher.Invoke(() =>
                {
                    Text = text;
                    Highlight = HighlightingManager.Instance.GetDefinition("markdown");
                    Tokens = _counter.CalcTokenCount(Text).ToString();
                    _buffer.Copy(Text);
                });
            });
        }
        #endregion

        #region Menu
        public ICommand OpenProjectCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        private void OpenProjectDialog(object? _)
        {
            string? rootPath = _dialogs.ShowOpenFolderDialog();
            if (rootPath == null)
                return;
            OpenProject(rootPath);
        }

        private void OpenProject(string rootPath)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(rootPath);

            Task.Run(async () =>
            {
                await _cs.SetRootDirectoryAsync(rootPath);
                var tree = await _fsm.GetTreeAsync();
                App.Current.Dispatcher.Invoke(() =>
                {
                    Explorer.ClearTreeState();
                    Explorer.LoadProject(tree, false);
                    Text = string.Empty;
                    _observer.StartObserving();

                    // Добавляем в список последних
                    _recentProjectsViewModel.AddRecentProject(rootPath, name);
                });
            });
        }

        private void OpenSettings(object? e)
        {
            var window = new SettingsWindow(new SettingsWindowVM(_cs, _dialogs));
            window.ShowDialog();
        }
        #endregion

        #region Explorer
        public ExplorerVM Explorer { get; set; }
        public async Task OpenDocument(string path)
        {
            Text = await _fsm.ReadFileAsync(path);
            Highlight = _highlightSelector.SelectHighlight(path) ?? HighlightingManager.Instance.GetDefinition("markdown");
        }
        #endregion
    }
}
