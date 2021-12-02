using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Wallpaper
{
    public class WallpaperCollection
    {
        public WallpaperCollection(string path)
        {
            FolderPath = new Uri(path, UriKind.Absolute);
            var directory = new DirectoryInfo(FolderPath.AbsolutePath);
            List<FileInfo> files = new List<FileInfo>();
            List<Wallpaper> wallpapers = new List<Wallpaper>();
            foreach (var file in directory.GetFiles())
            {
                try
                {
                    if (file.IsImage())
                    {
                        //  See if it's actually an image.
                        var image = Image.FromFile(file.FullName);
                        files.Add(file);
                        wallpapers.Add(new Wallpaper()
                        {
                            Filename = file.Name,
                            FullPath = file.FullName,
                            Resolution = (image.Width, image.Height)
                        });
                    }
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }

            Wallpapers = wallpapers;


        }
        public Uri FolderPath { get; set; }
        public IEnumerable<Wallpaper> Wallpapers;

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