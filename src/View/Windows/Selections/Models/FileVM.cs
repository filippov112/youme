using System;
using System.Collections.Generic;
using System.Text;
using View.Other;
using View.Windows.Selections.Explorer;

namespace View.Windows.Selections.Models
{
    public class FileVM: ViewModel
    {
        public string Path { get; set; } = string.Empty;
        public ExplorerItemVM? Item { get; set; }
        public bool IsExist => Item is not null;
    }
}
