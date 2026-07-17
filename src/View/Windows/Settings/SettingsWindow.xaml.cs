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
    }
}
