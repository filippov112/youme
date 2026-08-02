using System.Windows;

namespace View.Windows.FileComponents
{
    /// <summary>
    /// Логика взаимодействия для FileComponentsWindow.xaml
    /// </summary>
    public partial class FileComponentsWindow : Window
    {
        private FileComponentsWindowVM fileComponentsWindowVM;

        public FileComponentsWindow(FileComponentsWindowVM fileComponentsWindowVM)
        {
            this.fileComponentsWindowVM = fileComponentsWindowVM;
            DataContext = fileComponentsWindowVM;
            InitializeComponent();
        }
    }
}
