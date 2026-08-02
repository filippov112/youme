using System.ComponentModel;
using System.Windows;

namespace View.Windows.Selections
{
    /// <summary>
    /// Логика взаимодействия для SelectionSettingsWindow.xaml
    /// </summary>
    public partial class SelectionSettingsWindow : Window
    {
        private readonly SelectionSettingsWindowVM _vm;
        public SelectionSettingsWindow(SelectionSettingsWindowVM vm)
        {
            _vm = vm;
            InitializeComponent();
            DataContext = vm;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_vm.ApproveClosing())
                base.OnClosing(e);
            else
                e.Cancel = true;
        }
    }
}
