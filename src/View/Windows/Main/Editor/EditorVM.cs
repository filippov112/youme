using ICSharpCode.AvalonEdit.Highlighting;
using View.Other;
using View.Windows.Main.Editor.Services;

namespace View.Windows.Main.Editor
{
    public class EditorVM : ViewModel
    {
        private readonly IHighlightSelector _selector;
        public EditorVM(IHighlightSelector selector)
        {
            _selector = selector;
        }

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

        public void SetHighlight(string path)
        {
            Highlight = _selector.SelectHighlight(path) ?? HighlightingManager.Instance.GetDefinition("markdown");
        }
    }
}
