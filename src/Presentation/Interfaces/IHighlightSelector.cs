using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Presentation.Interfaces
{
    internal interface IHighlightSelector
    {
        public IHighlightingDefinition? SelectHighlight(string filePath);
    }
}
