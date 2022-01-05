using System;
using System.Collections.Generic;

namespace Wallpaper.Options
{
    public class WallpaperSettings
    {
        public string BackgroundColor { get; set; } = "";
        public IEnumerable<string> WallpaperFolders { get; set; }   //  Folders where wallpapers are stored.
        public TimeSpan WallpaperLifetime { get; set; } //  Dont' switch if 0.
    }
}