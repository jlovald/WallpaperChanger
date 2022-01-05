using System.IO;
using Microsoft.Extensions.Options;
using Wallpaper.Options;

namespace WallpaperMonitorService
{
    public static class OptionsValidator
    {

        public static void Validate(WallpaperSettings wallpaperSettings)
        {
            ValidateWallpaperSettings(wallpaperSettings);
        }

        public static void Validate(IOptions<WallpaperSettings> wallpaperSettings)
        {
            ValidateWallpaperSettings(wallpaperSettings.Value);
        }

        private static void ValidateWallpaperSettings(WallpaperSettings wallpaperSettings)
        {
            //  Check directories.
            foreach (var path in wallpaperSettings.WallpaperFolders)
            {
                if (!Directory.Exists(path))
                {
                    throw new DirectoryNotFoundException($"{path} is not a valid directory.");
                }
            }
        }
    }
}
