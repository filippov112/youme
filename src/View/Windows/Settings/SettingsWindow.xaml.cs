using System.ComponentModel;
using System.Configuration;
using System.Windows;

namespace View.Windows.Settings
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly SettingsWindowVM _vm;
        public SettingsWindow(SettingsWindowVM vm)
        {
            _vm = vm;
            InitializeComponent();
            DataContext = vm;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_vm.OnCloced())
                base.OnClosing(e);
            else
                e.Cancel = true;
        }
    }
}
