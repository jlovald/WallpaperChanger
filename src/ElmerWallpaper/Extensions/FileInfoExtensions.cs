using System.IO;
using System.Linq;

namespace Wallpaper.Extensions
{
    public static class FileInfoExtensions
    {
        public static bool IsImage(this FileInfo file)
        {
            var allowedExtensions = new[] { ".jpg", ".png", ".gif", ".jpeg" };
            return allowedExtensions.Contains(file.Extension.ToLower());
        }
    }
}