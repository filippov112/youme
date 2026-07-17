using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using View.Windows.Main;

namespace View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private readonly IServiceProvider _provider;
        public App()
        {
            _provider = DependencyInjection.AddServices();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var mainWindow = _provider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
