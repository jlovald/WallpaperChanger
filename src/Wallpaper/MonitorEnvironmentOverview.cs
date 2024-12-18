using System.Collections.Generic;
using System.Drawing;

namespace Wallpaper
{
    public class MonitorEnvironmentOverview
    {
        public Rectangle Dimensions { get; set; }
        public IEnumerable<TrimmedScreen> Screens { get; set; }

    }
}