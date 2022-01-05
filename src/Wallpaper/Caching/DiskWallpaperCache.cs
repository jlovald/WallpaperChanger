using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Wallpaper
{
    public class DiskWallpaperCache : IWallpaperCache
    {

        private readonly FolderConfiguration _folderSettings;
        private List<WallpaperCacheEntry> _wallpaperCache = new List<WallpaperCacheEntry>();
        private ILogger<DiskWallpaperCache> _logger;

        public DiskWallpaperCache(IOptions<FolderConfiguration> folderSettings, ILogger<DiskWallpaperCache> logger)
        {
            _folderSettings = folderSettings.Value;
            _logger = logger;
        }

        public DiskWallpaperCache(FolderConfiguration folderSettings)
        {
            _folderSettings = folderSettings;
        }

        public async Task<List<WallpaperCacheEntry>> GetCurrentWallpapers()
        {
            if (_wallpaperCache == null || ! _wallpaperCache.Any())
            {
                List<WallpaperCacheEntry> wallpaperCache = null;
                var content = "";
                try
                {
                    
                    using var sr = new StreamReader(_folderSettings.GetCachePath());
                    content = await sr.ReadToEndAsync();
                    wallpaperCache = (JsonSerializer.Deserialize<WallpaperCacheEntry[]>(content)).ToList();

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while reading from cache.");
                }
                //  Check disk.
                if (wallpaperCache == null)
                {
                    return new List<WallpaperCacheEntry>();
                }

                return wallpaperCache;
                //  If not found return null.
                //  if found return stuff.



            }
            //  Bla bla serialize from disk?
            return _wallpaperCache;
        }

        public async Task SetCurrentWallpaperState(List<WallpaperCacheEntry> currentWallpapers)
        {
            _wallpaperCache = currentWallpapers;
            try
            {
                var currentWallpaperJson = JsonSerializer.Serialize(currentWallpapers.ToArray());
                await using var fw = File.Open(_folderSettings.GetCachePath(), FileMode.Create);
                await using var sw = new StreamWriter(fw);
                await sw.WriteAsync(currentWallpaperJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while setting currentWallpapers");
            }
            //  write to disk?
        }
    }
}