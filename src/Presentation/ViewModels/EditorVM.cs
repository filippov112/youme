using ICSharpCode.AvalonEdit.Highlighting;
using Presentation.Other;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class EditorVM: ViewModel
    {
        public EditorVM()
        {
            SaveCommand = new RelayCommand(OnSave);
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

        // Save
        public event Action<string>? TextSaved;
        public ICommand SaveCommand { get; set; }
        private void OnSave(object? _)
        {
            TextSaved?.Invoke(Text);
        }
    }
}
