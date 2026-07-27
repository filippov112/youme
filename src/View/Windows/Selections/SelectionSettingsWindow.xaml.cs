using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using View.Windows.Settings;

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
