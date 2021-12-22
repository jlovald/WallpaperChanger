using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wallpaper.Extensions;
using Wallpaper.Options;

namespace Wallpaper
{
    public class WallpaperEngine
    {
        private static readonly uint SPI_SETDESKWALLPAPER = 0x14;
        private static readonly uint SPIF_UPDATEINIFILE = 0x01;
        private static readonly uint SPIF_SENDWININICHANGE = 0x02;
        private readonly IDisplaySettingsCache _displaySettingsCache;
        private readonly FolderConfiguration _folderSettings;
        private readonly ILogger<WallpaperEngine> _logger;
        private readonly IWallpaperCache _wallpaperCache;
        private readonly WallpaperCollection _wallpaperCollection;

        private readonly WallpaperSettings _wallpaperSettings;

        private static int changes = 0;

        public WallpaperEngine(IOptions<WallpaperSettings> wallpaperSettings, WallpaperCollection wallpaperCollection,
            ILogger<WallpaperEngine> logger, IDisplaySettingsCache displaySettingsCache,
            IOptions<FolderConfiguration> folderSettings, IWallpaperCache wallpaperCache)
        {
            _wallpaperSettings = wallpaperSettings.Value;
            _wallpaperCollection = wallpaperCollection;
            _logger = logger;
            _displaySettingsCache = displaySettingsCache;
            _folderSettings = folderSettings.Value;
            _wallpaperCache = wallpaperCache;
        }

        public MonitorEnvironmentOverview GetMonitorsAndDimensions()
        {
            var a = new MonitorEnvironmentOverview();
            a.Dimensions = GetBounds(_displaySettingsCache.GetDisplaySettings());
            a.Screens = _displaySettingsCache.GetDisplaySettings().Select(s => new TrimmedScreen()
            {
                Bounds = s.Bounds,
                DeviceName = s.DeviceName,
                IsPrimaryDisplay = s.Primary
            });
            return a;
        }


        public async Task<string> CreateRandomWallpaper()
        {
            changes++;
            var overview = GetMonitorsAndDimensions();
            var wallpapers = _wallpaperCollection;
            var canvas = new Bitmap(overview.Dimensions.Width, overview.Dimensions.Height);
            var g = Graphics.FromImage(canvas);
            g.Clear(SystemColors.AppWorkspace);
            var wallpaperCache = new List<WallpaperCacheEntry>();
            foreach (var screen in overview.Screens)
            {
                //  Get wallpaper for given resolution
                Wallpaper wallpaper = null;
                var cachedWallpaperEntry = (await _wallpaperCache.GetCurrentWallpapers()).FirstOrDefault(e =>
                    e.Screen.DeviceName == screen.DeviceName &&
                    e.Screen.Bounds.Height == screen.Bounds.Height &&
                    e.Screen.Bounds.Width == screen.Bounds.Width); //  TODO: Fix scaling etc.
                var cachedWallpaper = cachedWallpaperEntry?.Wallpaper; //  use later for some logic
                if (cachedWallpaperEntry != null && cachedWallpaperEntry.Expiration > DateTime.UtcNow)
                {
                    wallpaper = cachedWallpaperEntry.Wallpaper;
                }
                else
                {
                    //  use massive brain.
                    wallpaper = wallpapers.GetWallpapers(screen.Bounds.Width, screen.Bounds.Height).Randomize()
                        .FirstOrDefault();
                }
                  


                _logger.LogInformation("Got the following wallpaper ", wallpaper);

                if (wallpaper == null) continue;
                var img = Image.FromFile(wallpaper.FullPath);

                //  calculate x and y based on origin.
                var x = screen.Bounds.X + Math.Abs(overview.Dimensions.X);
                var y = screen.Bounds.Y + Math.Abs(overview.Dimensions.Y);
                g.DrawImage(img, x, y, img.Width, img.Height);
                wallpaperCache.Add(
                    new WallpaperCacheEntry(DateTime.UtcNow.Add(_wallpaperSettings.WallpaperLifetime), screen,
                        wallpaper));
                img.Dispose();
            }
            await _wallpaperCache.SetCurrentWallpapers(wallpaperCache);

            g.Dispose();

            var fullPath = _folderSettings.GetTmpImagePath();
            _logger.LogInformation($"Saving new wallpaper to: {fullPath}");

            canvas.Save(fullPath, ImageFormat.Bmp);
            canvas.Dispose();


            SetBackground(fullPath);
            return fullPath; //  TODO: Use Cache to determine wallpapers
            //  TODO use appdata or something like that instead.
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
        private static extern int SystemParametersInfo(uint action, uint uParam, string vParam, uint winIni);
    }
}