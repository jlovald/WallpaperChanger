using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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



        /// <summary>
        /// Generates a new wallpaper and sets it as the new background.
        /// </summary>
        /// <returns> path of temporary wallpaper</returns>
        public async Task<string> GenerateWallpaper()
        {
            var tmpImagePath = _folderSettings.GetTmpImagePath();
            
            var monitorOverview = GetMonitorsAndDimensions();
            var (bitmap, graphics) = GenerateCanvas(monitorOverview, tmpImagePath);
            var wallpaperSateCache = new List<WallpaperCacheEntry>();
            foreach (var screen in monitorOverview.Screens)
            {
                Wallpaper wallpaper;
                //  Check for existing wallpaper state for matching monitors.
                var cachedWallpaperEntry = (await _wallpaperCache.GetCurrentWallpapers()).FirstOrDefault(e =>
                    e.Screen.DeviceName == screen.DeviceName &&
                    e.Screen.Bounds.Height == screen.Bounds.Height &&
                    e.Screen.Bounds.Width == screen.Bounds.Width); //  TODO: Fix scaling etc.

                if (cachedWallpaperEntry != null && cachedWallpaperEntry.Expiration > DateTime.UtcNow)
                {
                    wallpaper = cachedWallpaperEntry.Wallpaper;
                }
                else
                {
                    wallpaper = _wallpaperCollection.GetWallpapers(screen.Bounds.Width, screen.Bounds.Height).Randomize()
                        .FirstOrDefault();
                }
                _logger.LogInformation($"Got the the wallpaper: '{wallpaper?.FullPath}' for the monitor: {screen.DeviceName}");

                wallpaperSateCache.Add(
                    new WallpaperCacheEntry(DateTime.UtcNow.Add(_wallpaperSettings.WallpaperLifetime), screen,
                        wallpaper));
                if (wallpaper == null) continue;

                using var img = Image.FromFile(wallpaper.FullPath);
                var (x, y) = CalculateMonitorOriginCoordinates(monitorOverview, screen);
                graphics.DrawImage(img, x, y, img.Width, img.Height);

                
                img.Dispose();
            }

            await _wallpaperCache.SetCurrentWallpaperState(wallpaperSateCache);

            _logger.LogInformation($"Saving new wallpaper to: {tmpImagePath}");

            bitmap.Save(tmpImagePath, ImageFormat.Bmp);
            graphics.Dispose();
            bitmap.Dispose();
            SetBackground(tmpImagePath);
            return tmpImagePath;

        }



        private (int x, int y) CalculateMonitorOriginCoordinates(MonitorEnvironmentOverview monitorOverview, TrimmedScreen screen)
        {
            return (screen.Bounds.X + Math.Abs(monitorOverview.Dimensions.X),
                screen.Bounds.Y + Math.Abs(monitorOverview.Dimensions.Y));
        }

        private (Image bitmap, Graphics graphics) GenerateCanvas(MonitorEnvironmentOverview overview,
            string existingBackgroundPath)
        {
            var bitmap = new Bitmap(overview.Dimensions.Width, overview.Dimensions.Height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(SystemColors.AppWorkspace);

            return (bitmap, graphics);
        }

        private static void SetBackground(string path)
        {
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

        private MonitorEnvironmentOverview GetMonitorsAndDimensions() => new MonitorEnvironmentOverview
        {
            Dimensions = GetBounds(_displaySettingsCache.GetDisplaySettings()),
            Screens = _displaySettingsCache.GetDisplaySettings().Select(s => new TrimmedScreen()
            {
                Bounds = s.Bounds,
                DeviceName = s.DeviceName,
                IsPrimaryDisplay = s.Primary
            })
        };
       

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(uint action, uint uParam, string vParam, uint winIni);
    }
}