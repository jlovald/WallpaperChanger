using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Wallpaper
{
    public class MonitorEnvironmentOverview
    {
        public Rectangle Dimensions { get; set; }
        public IEnumerable<TrimmedScreen> Screens { get; set; }

    }
}