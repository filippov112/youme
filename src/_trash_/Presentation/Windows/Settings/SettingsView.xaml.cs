using Application.Services;
using Infrastructure.Services;
using System.Windows;

namespace Presentation.Windows.Settings
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        private readonly SettingsVM vm;
        private readonly IStorageService _storageService;
        public SettingsView(IStorageService storageService)
        {
            _storageService = storageService;
            InitializeComponent();
            vm = new SettingsVM(storageService);
            DataContext = vm;
            btnProjectSettings.IsEnabled = !string.IsNullOrEmpty(storageService.ProjectFolder);
        }

        private void DeactivateAllPanels()
        {
            generalSettings.Visibility = Visibility.Hidden;
            projectSettings.Visibility = Visibility.Hidden;
        }

        private void btnProjectSettings_Click(object sender, RoutedEventArgs e)
        {
            DeactivateAllPanels();
            projectSettings.Visibility = Visibility.Visible;
        }

        private void btnGeneralSettings_Click(object sender, RoutedEventArgs e)
        {
            DeactivateAllPanels();
            generalSettings.Visibility = Visibility.Visible;
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            _storageService.SaveSettings(vm.GlobalConfig, vm.LocalConfig);
            vm.BtnSaveIsActive = false;
        }
    }
}
