using Application.Interfaces;
using ICSharpCode.AvalonEdit.Highlighting;
using Presentation.Interfaces;
using Presentation.Other;
using Presentation.Windows;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class MainWindowVM: ViewModel
    {
        private readonly IDialogService _dialogs;
        private readonly IConfigService _cs;
        private readonly IFileSystemManager _fsm;
        private readonly SettingsWindow _settings;
        private readonly IPromptBuilder _promptBuilder;
        private readonly ITokenCounter _counter;
        private readonly IBufferExchange _buffer;
        private readonly ICatalogChangedHandler _catalogChanged;
        private readonly IHighlightSelector _highlightSelector;
        public MainWindowVM(            
            ExplorerVM explorer, 
            IDialogService dialogs, 
            IConfigService cs, 
            IFileSystemManager fsm, 
            SettingsWindow settings,
            IPromptBuilder promptBuilder,
            ITokenCounter counter,
            IBufferExchange buffer,
            ICatalogChangedHandler catalogChanged,
            IHighlightSelector highlight
            )
        {
            _highlightSelector = highlight;
            _buffer = buffer;
            _counter = counter;
            _promptBuilder = promptBuilder;
            _settings = settings;
            Explorer = explorer;
            Explorer.OpenFile = async (string path) => { await OpenDocument(path); };
            _catalogChanged = catalogChanged;
            _catalogChanged.CatalogChanged += async () => { Explorer.LoadProject(await fsm.GetTreeAsync()); };
            _dialogs = dialogs;
            _cs = cs;
            _fsm = fsm;
            OpenProjectCommand = new RelayCommand(_ => { Task.Run(OpenProject); });
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            BuildPromptCommand = new RelayCommand(_ => { Task.Run(BuildPrompt); });
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
        private async Task BuildPrompt()
        {
            var files = new List<string>();
            if (Explorer.Items.Count > 0)
            {
                Explorer.Items[0].GetSelectedFiles(files);
            }
            Text = await _promptBuilder.GetPrompt(files, Query);
            Highlight = HighlightingManager.Instance.GetDefinition("markdown");
            Tokens = _counter.CalcTokenCount(Text).ToString();
            _buffer.Copy(Text);
        }
        #endregion


        #region Menu
        public ICommand OpenProjectCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        private async Task OpenProject()
        {
            string? rootPath = _dialogs.ShowOpenFolderDialog();
            if (rootPath == null)
                return;

            await _cs.SetRootDirectoryAsync(rootPath);
            Explorer.LoadProject(await _fsm.GetTreeAsync());
            Text = string.Empty;
        }

        private void OpenSettings(object? e)
        {
            _settings.ShowDialog();
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
