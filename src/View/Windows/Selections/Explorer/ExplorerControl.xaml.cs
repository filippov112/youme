using UserControl = System.Windows.Controls.UserControl;

namespace View.Windows.Selections.Explorer
{
    /// <summary>
    /// Логика взаимодействия для Explorer.xaml
    /// </summary>
    public partial class ExplorerControl : UserControl
    {
        private ExplorerVM VM => (ExplorerVM)DataContext;


        public ExplorerControl()
        {
            InitializeComponent();
        }


    }
}
