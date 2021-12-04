using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wallpaper;
using WallpaperMonitorService.Extensions;

namespace WallpaperMonitorService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Setting background on launch.");
            host.Services.GetRequiredService<ResolutionMonitorHostedService>().SetWallpaper();
            logger.LogInformation("Starting service.");
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging();
                    services.ConfigureConfiguration(context.Configuration);
                    services.AddSingleton<WallpaperCollection>();
                    services.AddSingleton<WallpaperEngine>();
                    services.AddSingleton<ResolutionMonitorHostedService>();
                    services.AddHostedService<ResolutionMonitorHostedService>();
                });
    }
}
