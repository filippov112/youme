using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pdp.App;
using Pdp.Inf;
using Pdp.UI.Controls;
using Pdp.UI.Interfaces;
using Pdp.UI.Services;
using Pdp.UI.ViewModels;
using Pdp.UI.Windows;


namespace Pdp.UI
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
