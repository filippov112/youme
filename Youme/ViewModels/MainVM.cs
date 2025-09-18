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
    }
}
