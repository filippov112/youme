using Core.Explorer.Services;
using Core.PromptBuilder.Service;
using Core.RecentProjects.Services;
using Core.Settings.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this ServiceCollection services)
        {

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddSingleton<IConfigService, ConfigService>();
            services.AddSingleton<IPromptService, PromptService>();
            services.AddSingleton<ICatalogChangeEvent, CatalogChangedHandler>();
            services.AddSingleton<IRecentProjectsService, RecentProjectsService>();
        }
    }
}
