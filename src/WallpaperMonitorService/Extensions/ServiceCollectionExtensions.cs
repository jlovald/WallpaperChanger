using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wallpaper.Options;

namespace WallpaperMonitorService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<WallpaperSettings>(options => configuration.GetSection("WallpaperSettings").Bind(options));
            var sb = services.BuildServiceProvider();
            var settings = sb.GetRequiredService<IOptions<WallpaperSettings>>();
            OptionsValidator.Validate(sb.GetRequiredService<IOptions<WallpaperSettings>>());
            

            return services;
        }
    }
}