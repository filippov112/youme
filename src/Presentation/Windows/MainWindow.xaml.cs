using Presentation.ViewModels;
using System.Windows;
using DragEventArgs = System.Windows.DragEventArgs;

namespace Presentation.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowVM _vm;
        public MainWindow(MainWindowVM vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = vm;
        }

        #region Перенос элементов дерева в промпт

        /// <summary>
        /// Обработчик сброса элемента дерева в текстовое поле промпта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FilePath"))
            {
                if (e.Data.GetData("FilePath") is string filePath)
                    _vm.Query = filePath;
                e.Handled = true;
            }
        }
        #endregion

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _vm.Search = ((System.Windows.Controls.TextBox)sender).Text;
        }
    }
}
