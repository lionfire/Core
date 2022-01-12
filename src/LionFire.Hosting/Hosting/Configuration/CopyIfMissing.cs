using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace LionFire.Hosting
{
    public static class LionFireHostingConfigurationUtils // RENAME CopyIfMissingUtils or something
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

        public static void OverwriteFromBaseDirectory(string src, string dest = null, bool onlyIfNonExampleMissing = true)
        {
            dest ??= src;
            var srcPath = Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory), src);

            if (onlyIfNonExampleMissing && !File.Exists(dest) && !File.Exists(dest.Replace("example.", "")))
            {
                return;
            }

            if (File.Exists(dest)) { File.Delete(dest); }
            if (File.Exists(srcPath))
            {
                File.Copy(srcPath, dest);
            }
        }
    }
}

