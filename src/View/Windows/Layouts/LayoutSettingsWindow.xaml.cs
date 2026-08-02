using System.Windows;

namespace View.Windows.Layouts
{
    /// <summary>
    /// Логика взаимодействия для LayoutSettingsWindow.xaml
    /// </summary>
    public partial class LayoutSettingsWindow : Window
    {
        public LayoutSettingsWindow(LayoutSettingsWindowVM layoutSettingsWindowVM)
        {
            LayoutSettingsWindowVM = layoutSettingsWindowVM;
            DataContext = layoutSettingsWindowVM;
            InitializeComponent();
        }

        public LayoutSettingsWindowVM LayoutSettingsWindowVM { get; }
    }
}
