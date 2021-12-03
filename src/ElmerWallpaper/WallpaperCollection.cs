using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Wallpaper.Extensions;
using Wallpaper.Options;

namespace Wallpaper
{
    public class WallpaperCollection
    {
        private List<Wallpaper> _wallpapers { get;} = new List<Wallpaper>();

        private WallpaperSettings _wallpaperSettings { get; set; }
        public WallpaperCollection(IOptions<WallpaperSettings> wallpaperSettings)
        {
            _wallpaperSettings = wallpaperSettings.Value;
            var dirs = _wallpaperSettings.WallpaperFolders.Select(path =>
            {
                if (!Directory.Exists(path))
                {
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
                        _wallpapers.Add(new Wallpaper()
                        {
                            Filename = file.Name,
                            FullPath = file.FullName,
                            Resolution = (image.Width, image.Height)
                        });
                        image.Dispose();
                    }
                    catch (Exception ex)
                    {
                        // ignored
                    }
                }
            }
        }


        public IEnumerable<Wallpaper> GetWallpapers(int width, int height)
        {
            return _wallpapers.Where(w => w.Resolution.width == width && w.Resolution.height == height);
        }
    }

    public class Wallpaper
    {
        public (int width, int height) Resolution { get; set; }
        public string Filename { get; set; }
        public string FullPath { get; set; }
    }
   
}