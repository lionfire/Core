using System.Collections.Generic;
using System.Linq;
using System.IO;
using LionFire.Instantiating;
using LionFire.Persistence;

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

        public string GetPath<T>(string assetSubpath = null, PersistenceContext context = null)
        {
            return Path.Combine(context?.RootPath ?? RootDir, AssetPathUtils.GetSubpath<T>(assetSubpath, context)) + (assetSubpath == null ? "" : FileExtensionWithDot); ;
        }
        public string GetPath(object obj, string assetSubpath = null, PersistenceContext context = null)
        {
            return Path.Combine(context?.RootPath ?? RootDir, AssetPathUtils.GetSubpath(obj, assetSubpath, context)) + (assetSubpath == null ? "" : FileExtensionWithDot); ;
        }

        public abstract string FileExtension { get; }
        public string FileExtensionWithDot { get { return (string.IsNullOrWhiteSpace(FileExtension) ? "" : "." + FileExtension); } }


        public IEnumerable<string> Find<T>(string searchString = null, PersistenceContext context = null)
        {
            var dir = GetPath<T>(context: context);
            if (searchString == null) searchString = "*";
            foreach (var path in Directory.GetFiles(dir, searchString + FileExtensionWithDot))
            {
                var assetName = path.Replace(dir, "").TrimStart('/').TrimStart('\\').Replace(FileExtensionWithDot, "");
                yield return assetName;
            }
        }
    }
}
