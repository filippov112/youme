using ICSharpCode.AvalonEdit.Highlighting;

namespace Presentation.Interfaces
{
    public interface IHighlightSelector
    {
        public IHighlightingDefinition? SelectHighlight(string filePath);
    }
}
