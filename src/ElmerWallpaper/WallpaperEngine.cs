using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ElmerWallpaper;
using Microsoft.Extensions.Options;
using Wallpaper.Extensions;
using Wallpaper.Options;

namespace Wallpaper
{
    public class WallpaperEngine
    {

        private readonly WallpaperSettings _wallpaperSettings;
        private readonly WallpaperCollection _wallpaperCollection;

        public WallpaperEngine(IOptions<WallpaperSettings> wallpaperSettings, WallpaperCollection wallpaperCollection)
        {
            _wallpaperSettings = wallpaperSettings.Value;
            _wallpaperCollection = wallpaperCollection;
        }

        public MonitorEnvironmentOverview GetMonitorsAndDimensions()
        {
            var screens = Screen.AllScreens;
            return GetMonitorsAndDimensions(screens);
        }

        public MonitorEnvironmentOverview GetMonitorsAndDimensions(Screen[] screens)
        {
            return new MonitorEnvironmentOverview()
            {
                Dimensions = GetBounds(screens),
                Screens = screens,
            };
        }
        

        public string CreateRandomWallpaper(WallpaperCollection wallpapers, MonitorEnvironmentOverview overview)
        {
            //  TODO use appdata or something like that instead.
            var path = ExecutingAssemblyContext.AssemblyDirectory;

            var canvas = new Bitmap(overview.Dimensions.Width, overview.Dimensions.Height);
            var g = Graphics.FromImage(canvas);
            g.Clear(SystemColors.AppWorkspace);
    
            foreach (var screen in overview.Screens)
            {
                //  Get wallpaper for given resolution
                var wallpaper = (wallpapers.GetWallpapers(screen.Bounds.Width, screen.Bounds.Height).Randomize())
                    .FirstOrDefault();
                if(wallpaper == null) continue;
                var img = Image.FromFile(wallpaper.FullPath);

                //  calculate x and y based on origin.
                var x = (screen.Bounds.X +Math.Abs(overview.Dimensions.X));
                var y = (screen.Bounds.Y + Math.Abs(overview.Dimensions.Y));
                g.DrawImage(img, x, y, img.Width, img.Height);
                img.Dispose();
            }

            g.Dispose();
            var guid = Guid.NewGuid();
            var fullPath = path + $"\\{guid}.bmp";
            canvas.Save(fullPath, System.Drawing.Imaging.ImageFormat.Bmp);
            canvas.Dispose();


            SetBackground(fullPath);
            return fullPath;
        }

        public string CreateRandomWallpaper()
        {
            var monitorOverview = GetMonitorsAndDimensions();
            //  TODO use appdata or something like that instead.
            var path = ExecutingAssemblyContext.AssemblyDirectory;

            var canvas = new Bitmap(monitorOverview.Dimensions.Width, monitorOverview.Dimensions.Height);
            var g = Graphics.FromImage(canvas);
            g.Clear(SystemColors.AppWorkspace);

            foreach (var screen in monitorOverview.Screens)
            {
                //  Get wallpaper for given resolution
                var wallpaper = _wallpaperCollection.GetWallpapers(screen.Bounds.Width, screen.Bounds.Height).Randomize()
                    .FirstOrDefault();
                if (wallpaper == null) continue;
                var img = Image.FromFile(wallpaper.FullPath);

                //  calculate x and y based on origin.
                var x = (screen.Bounds.X + Math.Abs(monitorOverview.Dimensions.X));
                var y = (screen.Bounds.Y + Math.Abs(monitorOverview.Dimensions.Y));
                g.DrawImage(img, x, y, img.Width, img.Height);
                img.Dispose();
            }

            g.Dispose();
            var guid = Guid.NewGuid();
            var fullPath = path + $"\\{guid}.bmp";
            canvas.Save(fullPath, System.Drawing.Imaging.ImageFormat.Bmp);
            canvas.Dispose();


            SetBackground(fullPath);
            return fullPath;
        }


        private void SetBackground(string path)
        {
            //  Todo make this more readable
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, 0x0001);
        }

        private static Rectangle GetBounds(IEnumerable<Screen> rects)
        {
            var rectangles = rects.Select(r => r.Bounds).ToList();
            var xMin = rectangles.Min(s => s.X);
            var yMin = rectangles.Min(s => s.Y);
            var xMax = rectangles.Max(s => s.X + s.Width);
            var yMax = rectangles.Max(s => s.Y + s.Height);
            return new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);
        private static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        private static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        private static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;

    }
   
}
