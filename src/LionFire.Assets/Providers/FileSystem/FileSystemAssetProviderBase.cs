using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace LionFire.Assets.Providers.FileSystem
{
    // FUTURE: Integrate with Serializer framework to try loading files based on extension

    public abstract class FileSystemAssetProviderBase
    {
        public string RootDir { get; set; }

        public FileSystemAssetProviderBase(string rootDir)
        {
            RootDir = rootDir;
        }

        public string GetPath<T>(string assetSubpath = null)
        {
            return Path.Combine(RootDir, AssetPathUtils.GetSubpath<T>(assetSubpath)) + (assetSubpath == null ? "" : FileExtensionWithDot); ;
        }

        public abstract string FileExtension { get; }
        public string FileExtensionWithDot { get { return (string.IsNullOrWhiteSpace(FileExtension) ? "" : "." + FileExtension); } }


        public IEnumerable<string> Find<T>(string searchString = null)
        {
            var dir = GetPath<T>();
            foreach (var path in Directory.GetFiles(dir, searchString + FileExtensionWithDot))
            {
                var assetName = path.Replace(dir, "").TrimStart('/').TrimStart('\\').Replace(FileExtensionWithDot, "");
                yield return assetName;
            }
        }

    }
}
