using Application;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Controls;
using Presentation.Interfaces;
using Presentation.Services;
using Presentation.ViewModels;
using Presentation.Windows;


namespace Presentation
{
    public static class DependencyInjection
    {
        public static IServiceProvider AddServices()
        {
            var services = new ServiceCollection();
            services.AddApplicationServices();
            services.AddInfrastructureServices();

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IHighlightSelector, HighlightSelector>();
            services.AddSingleton<IBufferExchange, BufferExchange>();

            // Explorer
            services.AddTransient<ExplorerVM>();
            services.AddTransient<Explorer>();

            // MainWindow
            services.AddTransient<MainWindowVM>();
            services.AddTransient<MainWindow>();

            // Settings
            services.AddTransient<SettingsWindowVM>();
            services.AddTransient<SettingsWindow>();

            return services.BuildServiceProvider();
        }
    }
}
