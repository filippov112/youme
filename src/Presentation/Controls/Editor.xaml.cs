using Presentation.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace Presentation.Controls
{
    /// <summary>
    /// Логика взаимодействия для Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        public Editor(EditorVM vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
