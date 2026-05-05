using ICSharpCode.AvalonEdit.Highlighting;
using Pdp.UI.Interfaces;
using System.IO;

namespace Pdp.UI.Services
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
}
