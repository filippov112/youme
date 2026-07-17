using Core;
using Inf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using View.Services;
using View.Windows.Main;
using View.Windows.Main.Editor;
using View.Windows.Main.Editor.Services;
using View.Windows.Main.Explorer;
using View.Windows.Main.RecentProjects;


namespace View
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
            // Editor
            services.AddTransient<EditorVM>();
            // Recent projects
            services.AddTransient<RecentProjectsVM>();

            // MainWindow
            services.AddTransient<MainWindowVM>();
            services.AddTransient<MainWindow>();

            return services.BuildServiceProvider();
        }
    }
}
