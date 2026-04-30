using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this ServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}
