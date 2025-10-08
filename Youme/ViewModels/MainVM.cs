using System.Diagnostics;
using Youme.ViewModels.Tree;

namespace Youme.ViewModels
{
    public class MainVM: ViewModel
    {
        private MainWindow view;
        public MainVM(MainWindow window) 
        {
            this.view = window;
        }

        public TreeModel Project { get; set; } = new TreeModel();
        public SearchVM<TreeElement> Search { get; set; } = new SearchVM<TreeElement>(checkText: (item) => item.Text, displayText: (item) => item.FullPath, OpenFile);
       


        private static void OpenFile(TreeElement item)
        {
            Debug.WriteLine(item.FullPath);
        }

    }
}
