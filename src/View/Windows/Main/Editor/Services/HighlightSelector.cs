using ICSharpCode.AvalonEdit.Highlighting;
using System.IO;

namespace View.Windows.Main.Editor.Services
{
    public class HighlightSelector : IHighlightSelector
    {
        public IHighlightingDefinition? SelectHighlight(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension switch
            {
                ".cs" => HighlightingManager.Instance.GetDefinition("C#"),
                ".xml" or ".config" => HighlightingManager.Instance.GetDefinition("XML"),
                ".js" => HighlightingManager.Instance.GetDefinition("JavaScript"),
                ".html" => HighlightingManager.Instance.GetDefinition("HTML"),
                ".css" => HighlightingManager.Instance.GetDefinition("CSS"),
                ".py" => HighlightingManager.Instance.GetDefinition("Python"),
                _ => null,// Без подсветки
            };
        }
    }

    public interface IHighlightSelector
    {
        public IHighlightingDefinition? SelectHighlight(string filePath);
    }
}
