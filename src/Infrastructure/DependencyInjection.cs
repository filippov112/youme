using Application.Interfaces;
using Infrastructure.Constants;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this ServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddSingleton<IFileSystemManager, FileSystemManager>();
            services.AddSingleton<IFileSystemWrapper, FileSystemWrapper>();
            services.AddSingleton<IDirectoryInfoWrapper, DirectoryInfoWrapper>();
            services.AddSingleton<ITokenCounter, TokenCounter>();
            services.AddSingleton<IConfigLoader, ConfigLoader>();
            services.AddSingleton<IFileSystemConstants, FileSystemConstants>();
            services.AddSingleton<ICatalogObserver, CatalogObserver>();
            services.AddSingleton<IBufferExchange, BufferExchange>();
        }
    }
}
