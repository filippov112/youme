using Application;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            services.AddApplicationServices();
            services.AddInfrastructureServices();

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IHighlightSelector, HighlightSelector>();

            // Explorer
            services.AddTransient<ExplorerVM>();
            services.AddTransient<Explorer>();

            // MainWindow
            services.AddTransient<MainWindowVM>();
            services.AddTransient<MainWindow>();

            return services.BuildServiceProvider();
        }
    }
}
