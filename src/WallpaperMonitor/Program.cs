using System;
using System.Windows.Forms;
using ElmerWallpaper;

namespace WallpaperMonitor
{
    class Program
    {
        private const string wallpaperPath = "C:\\Users\\johlov\\Pictures\\TestWallpapers\\";
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var overview = Engine.GetMonitorsAndDimensions();
            var wallpapers = new WallpaperCollection(wallpaperPath);
            var filename = Engine.CreateRandomWallpaper(wallpapers, overview);
            Engine.SetBackground(filename);
            Console.WriteLine();
        }
    }
}
