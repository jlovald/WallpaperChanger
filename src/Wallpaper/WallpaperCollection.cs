using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wallpaper.Extensions;
using Wallpaper.Options;

namespace Wallpaper
{
    public class WallpaperCollection
    {
        private List<Wallpaper> Wallpapers { get;} = new List<Wallpaper>();
        private readonly ILogger<WallpaperCollection> _logger;
        private readonly WallpaperSettings _wallpaperSettings;

        public WallpaperCollection(IOptions<WallpaperSettings> wallpaperSettings, ILogger<WallpaperCollection> logger)
        {
            _wallpaperSettings = wallpaperSettings.Value;
            _logger = logger;
            logger.LogInformation($"Checking folders from {wallpaperSettings.Value.WallpaperFolders.Count()} folder(s): {string.Join(", ", (IEnumerable<string>) wallpaperSettings.Value.WallpaperFolders)}");

            var dirs = _wallpaperSettings.WallpaperFolders.Select(path =>
            {
                if (!Directory.Exists(path))
                {
                    _logger.LogError("Error checking path.", new DirectoryNotFoundException($"{path} is not a valid directory."));
                    throw new DirectoryNotFoundException($"{path} is not a valid directory.");
                }
                return new DirectoryInfo(path);
            });

            CacheImageInformation(dirs);
        }


        private void CacheImageInformation(IEnumerable<DirectoryInfo> directories)
        {
            foreach (var dir in directories)
            {
                List<FileInfo> files = new List<FileInfo>();
                foreach (var file in dir.GetFiles())
                {
                    try
                    {
                        if (!file.IsImage()) continue;
                        //  See if it's actually an image.
                        var image = Image.FromFile(file.FullName);
                        files.Add(file);
                        Wallpapers.Add(new Wallpaper()
                        {
                            Filename = file.Name,
                            FullPath = file.FullName,
                            Resolution = (image.Width, image.Height)
                        });
                        image.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error caching image information", ex);
                        // ignored
                    }
                }
            }
        }


        public IEnumerable<Wallpaper> GetWallpapers(int width, int height)
        {
            return Wallpapers.Where(w => w.Resolution.width == width && w.Resolution.height == height);
        }
    }

    public class Wallpaper
    {
        public (int width, int height) Resolution { get; set; }
        public string Filename { get; set; }
        public string FullPath { get; set; }
    }
   
}