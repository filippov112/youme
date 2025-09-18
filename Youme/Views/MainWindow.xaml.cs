using System.Windows;
using Youme.Services;
using Youme.ViewModels;
using Youme.Views;

namespace Youme
{
    public partial class MainWindow : Window
    {
        private MainVM vm;
        public MainWindow()
        {
            InitializeComponent();
            vm = new MainVM(this);
            DataContext = vm;
        }

        private void OpenProject(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Выберите проект";
                folderDialog.UseDescriptionForTitle = true;

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    StorageService.ProjectFolder = folderDialog.SelectedPath;
                    vm.Project.LoadProject(folderDialog.SelectedPath);
                }
            }
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            var settings = new Settings();
            settings.ShowDialog();
        }

        private void BuildPrompt(object sender, RoutedEventArgs e)
        {
            editorAvalon.Text = ContentBuilder.Build(vm.Project.AllItems.Where(x => x.IsSelected && x.Type == ViewModels.Tree.ItemType.File).Select(x => x.FullPath).ToList());
        }
    }
}