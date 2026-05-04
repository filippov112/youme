using Presentation.ViewModels;
using System.Windows;

namespace Presentation.Windows
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
