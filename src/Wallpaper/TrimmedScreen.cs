using System.Drawing;

namespace Wallpaper
{
    public class TrimmedScreen
    {
        public Rectangle Bounds { get; set; }
        public bool IsPrimaryDisplay { get; set; }
        public string DeviceName { get; set; }
    }
}