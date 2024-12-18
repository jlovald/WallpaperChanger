using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
                    services.AddSingleton<IDisplaySettingsCache, InMemoryDisplaySettingsCache>(x =>
                        new InMemoryDisplaySettingsCache(Screen.AllScreens));
                    services.AddSingleton<WallpaperCollection>();
                    services.AddSingleton<WallpaperEngine>();
                    services.AddSingleton<ResolutionMonitorHostedService>();
                    services.AddHostedService<ResolutionMonitorHostedService>();
                    //services.AddNewtonsoftJson();
                    var sb = services.BuildServiceProvider();
                    var folderSettings = sb.GetRequiredService<IOptions<FolderConfiguration>>().Value;
                    if (!folderSettings.DefinedConfigPath())
                    {
                        services.AddSingleton<IWallpaperCache, InMemoryWallpaperCache>();
                    }
                    else
                    {
                        services.AddSingleton<IWallpaperCache, DiskWallpaperCache>();
                    }
                });
    }
}
