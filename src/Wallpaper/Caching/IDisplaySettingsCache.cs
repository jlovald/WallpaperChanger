using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Wallpaper
{
    public interface IDisplaySettingsCache
    {
        bool HasChanged(IEnumerable<Screen> changes);
        IEnumerable<Screen> GetDisplaySettings();
    }
    public abstract class BaseDisplaySettingsCache : IDisplaySettingsCache
    {
        protected IEnumerable<Screen> DisplaySettings { get; set; }


        protected BaseDisplaySettingsCache(IEnumerable<Screen> displaySettings)
        {
            DisplaySettings = displaySettings;
        }

        public virtual bool HasChanged(IEnumerable<Screen> currentDisplaySettings)
        {
            var changes = currentDisplaySettings.ToList();
            if (changes.Count() != DisplaySettings.Count()) return true;    //  Display has been added or unplugged?
            foreach (var screen in changes)
            {
                var nameMatches = DisplaySettings.Where(e => e.DeviceName == screen.DeviceName);
                if (!nameMatches.Any())
                {
                    UpdateCache(changes);
                    return true;
                }

                var matchingScreen = nameMatches.FirstOrDefault();

                if (matchingScreen.Bounds != screen.Bounds)
                {
                    UpdateCache(changes);
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<Screen> GetDisplaySettings()
        {
            return DisplaySettings;
        }

        protected virtual void UpdateCache(IEnumerable<Screen> change)
        {
            DisplaySettings = change;
        }
    }

    public class InMemoryDisplaySettingsCache : BaseDisplaySettingsCache
    {

        public InMemoryDisplaySettingsCache(IEnumerable<Screen> screens) : base(screens)
        {

        }

    }
}