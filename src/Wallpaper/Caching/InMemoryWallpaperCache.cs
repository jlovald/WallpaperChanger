using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wallpaper
{
    public class InMemoryWallpaperCache : IWallpaperCache
    {

        private List<WallpaperCacheEntry> _cache;

        public async Task<List<WallpaperCacheEntry>> GetCurrentWallpapers()
        {
            return _cache;
        }

        public async Task SetCurrentWallpapers(List<WallpaperCacheEntry> currentWallpapers)
        {
            _cache = currentWallpapers;
        }
    }
}