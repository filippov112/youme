using Core.CatalogParser.Services;
using Core.Explorer.Services;
using Core.Settings.Services;
using Core.Tools;
using Inf.CatalogParser;
using Inf.Constants;
using Inf.Explorer;
using Inf.FileSystem;
using Inf.Settings;
using Inf.Tools;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Inf
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this ServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddSingleton<IFileSystemService, FileSystemManager>();
            services.AddSingleton<IFileSystemWrapper, FileSystemWrapper>();
            services.AddSingleton<IDirectoryInfoWrapper, DirectoryInfoWrapper>();
            services.AddSingleton<ITokenCounterTool, TokenCounter>();
            services.AddSingleton<IConfigLoader, ConfigLoader>();
            services.AddSingleton<IFileSystemConstants, SystemConstants>();
            services.AddSingleton<ICatalogObserver, CatalogObserver>();
            services.AddSingleton<IBufferExchangeTool, BufferExchange>();
            services.AddSingleton<ICatalogJsonProcessor, CatalogJsonProcessor>();
        }
    }
}
