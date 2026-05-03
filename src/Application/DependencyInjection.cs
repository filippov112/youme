using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this ServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddSingleton<ISearchService, SearchService>();
            services.AddSingleton<IConfigService, ConfigService>();
            services.AddSingleton<IPromptBuilder, PromptBuilder>();
            services.AddSingleton<ICatalogChangedHandler, CatalogChangedHandler>();
        }
    }
}
