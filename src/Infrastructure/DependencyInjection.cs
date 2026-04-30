using System.Text;
using Application.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

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
        }
    }
}
