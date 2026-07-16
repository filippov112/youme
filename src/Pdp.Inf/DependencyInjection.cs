using Microsoft.Extensions.DependencyInjection;
using Pdp.App.Interfaces;
using Pdp.Inf.Constants;
using Pdp.Inf.Interfaces;
using Pdp.Inf.Services;
using System.Text;

namespace Pdp.Inf
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
            services.AddSingleton<ICatalogJsonProcessor, CatalogJsonProcessor>();
        }
    }
}
