using Application;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;


namespace Presentation
{
    public static class DependencyInjection
    {
        public static IServiceProvider AddServices()
        {
            var services = new ServiceCollection();
            services.AddApplicationServices();
            services.AddInfrastructureServices();

            //services.AddTransient<ProjectView>();

            return services.BuildServiceProvider();
        }
    }
}
