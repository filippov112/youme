using Infrastructure;
using Microsoft.Extensions.DependencyInjection;


namespace Presentation
{
    public static class DependencyInjection
    {
        public static IServiceProvider AddServices()
        {
            var services = new ServiceCollection();
            services.AddInfrastructureServices();
            //services.AddTransient<ProjectView>();

            return services.BuildServiceProvider();
        }
    }
}
