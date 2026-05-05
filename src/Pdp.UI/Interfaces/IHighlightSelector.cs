using ICSharpCode.AvalonEdit.Highlighting;

namespace Pdp.UI.Interfaces
{
    public interface IHighlightSelector
    {
        public IHighlightingDefinition? SelectHighlight(string filePath);
    }
}
