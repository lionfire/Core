using System;
using System.IO;

namespace LionFire.Hosting
{
    public static class LionFireHostingConfigurationUtils
    {
        public static bool TryCopyIfMissing(string sourcePath, string destPath)
        {
            if (!File.Exists(destPath))
            {
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, destPath);
                    return true;
                }
                return false;
            }
            return true;
        }
        public static bool TryCopyFromBaseDirectoryIfMissing(string filename)
            => TryCopyIfMissing(Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory), filename), filename);
    }
}
