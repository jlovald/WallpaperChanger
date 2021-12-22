
namespace Wallpaper
{

    public class FolderConfiguration
    {
        public string TmpImagePath { get; set; }
        public string CachePath { get; set; }

        public string GetTmpImagePath()
        {
            return (string.IsNullOrEmpty(TmpImagePath) ? GetAssemblyPath() : TmpImagePath.TrimEnd('\\')) + "\\tmp.bmp";
        }

        public string GetCachePath()
        {
            return (string.IsNullOrEmpty(CachePath) ? GetAssemblyPath() : CachePath.TrimEnd('\\')) + "\\settings.json";
        }

        public bool DefinedImagePath() => !string.IsNullOrEmpty(TmpImagePath);
        public bool DefinedConfigPath() => !string.IsNullOrEmpty(TmpImagePath);
        private string GetAssemblyPath()
        {
            return ExecutingAssemblyContext.AssemblyDirectory;

        }

    }

}