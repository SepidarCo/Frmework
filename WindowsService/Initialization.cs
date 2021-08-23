using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sepidar.WindowsService
{
    public class Initialization
    {
        public static async Task CreateHostAndAddService<THostedService>() where THostedService : HostedService
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<THostedService>();
                });
            builder.UseWindowsService();
            await builder.Build().RunAsync();
        }
    }
}
