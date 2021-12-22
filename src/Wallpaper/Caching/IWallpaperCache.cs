using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Wallpaper
{

    public interface IWallpaperCache
    {
        //  Pair montior by name and path to wallpaper.
        Task<List<WallpaperCacheEntry>> GetCurrentWallpapers();
        public Task SetCurrentWallpapers(List<WallpaperCacheEntry> currentWallpapers);

    }

    public class WallpaperCacheEntry
    {

        public WallpaperCacheEntry()
        {
            Expiration  = DateTime.MinValue;
            Screen = null;
            Wallpaper = null;
        }
        //  UTC.
        [JsonPropertyName("Expiration")]

        public DateTime Expiration { get; set; }

        [JsonPropertyName("Screen")]
        public TrimmedScreen Screen { get; set; }
        [JsonPropertyName("Wallpaper")]

        public Wallpaper Wallpaper { get; set; }

        public WallpaperCacheEntry(DateTime expiration, TrimmedScreen screen, Wallpaper wallpaper)
        {
            Expiration = expiration;
            Screen = screen;
            Wallpaper = wallpaper;
        }

        public bool HasExpired() => Expiration.ToUniversalTime() < DateTime.UtcNow;
    }
}