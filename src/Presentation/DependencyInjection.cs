using Application;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Controls;
using Presentation.Interfaces;
using Presentation.Services;
using Presentation.ViewModels;


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
            //services.AddTransient<ProjectView>();

            // Editor
            services.AddTransient<EditorVM>();
            services.AddTransient<Editor>();

            return services.BuildServiceProvider();
        }
    }
}
