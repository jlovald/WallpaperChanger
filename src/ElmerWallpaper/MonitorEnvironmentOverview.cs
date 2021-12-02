using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ElmerWallpaper
{
    public class MonitorEnvironmentOverview
    {
        public Rectangle Dimensions { get; set; }
        public IEnumerable<Screen> Screens { get; set; }
    }
}