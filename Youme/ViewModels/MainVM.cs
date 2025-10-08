using Youme.ViewModels.Tree;

namespace Youme.ViewModels
{
    public class MainVM: ViewModel
    {
        private MainWindow view;
        public MainVM(MainWindow window) 
        {
            this.view = window;
            Search.Tree = Project;
        }

        public TreeModel Project { get; set; } = new TreeModel();
        public TreeSearch Search { get; set; } = new TreeSearch(checkText: (item) => item.Text, displayText: (item) => item.FullPath);
    
    }
}
