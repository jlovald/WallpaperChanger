using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wallpaper;
using WallpaperMonitorService.Extensions;

namespace WallpaperMonitorService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging();
                    services.ConfigureConfiguration(context.Configuration);
                    services.AddSingleton<WallpaperCollection>();
                    services.AddSingleton<WallpaperEngine>();
                    services.AddHostedService<ResolutionMonitorHostedService>();
                });
    }
}
