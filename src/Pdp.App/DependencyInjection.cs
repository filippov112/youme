using Microsoft.Extensions.DependencyInjection;
using Pdp.App.Interfaces;
using Pdp.App.Services;
using System.Reflection;

namespace Pdp.App
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this ServiceCollection services)
        {

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddSingleton<IConfigService, ConfigService>();
            services.AddSingleton<IPromptBuilder, PromptBuilder>();
            services.AddSingleton<ICatalogChangedHandler, CatalogChangedHandler>();
            services.AddSingleton<IRecentProjectsService, RecentProjectsService>();
        }
    }
}
