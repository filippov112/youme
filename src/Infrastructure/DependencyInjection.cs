using System.Text;
using Application.Services;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this ServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IStorageService, StorageService>();
            services.AddSingleton<IContentBuilder, ContentBuilder>();
        }
    }
}
