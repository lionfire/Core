using System.Collections.Generic;
using System.Linq;
using System.IO;
using LionFire.Instantiating;
using LionFire.Persistence;
using LionFire.MultiTyping;
using System.Threading.Tasks;

namespace LionFire.Assets.Providers.FileSystem
{
    // FUTURE: Integrate with Serializer framework to try loading files based on extension

    public abstract class FileSystemAssetProviderBase
    {
        public string RootDir { get; set; }

        #region Construction and Initialization

        public FileSystemAssetProviderBase(string rootDir)
        {
            RootDir = rootDir;
        }

        protected void InitRootDir()
        {
            if (RootDir == null)
            {
                RootDir = LionFireEnvironment.AppProgramDataDir;
            }

            if (!Directory.Exists(RootDir))
            {
                Directory.CreateDirectory(RootDir);
            }
        }

        #endregion

        public void CreateDirIfNeeded(object obj, string assetSubpath = null, object context = null)
        {
            var dir = GetDir(obj, assetSubpath, context);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public string GetDir(object obj, string assetSubpath = null, object context = null)
        {
            var pc = context.ObjectAsType<PersistenceContext>();
            return Path.GetDirectoryName(Path.Combine(pc?.RootPath ?? RootDir, AssetPathUtils.GetSubpath(obj, assetSubpath, context)));
        }

        public string GetPath<T>(string assetSubpath = null, object context = null)
        {
            var pc = context.ObjectAsType<PersistenceContext>();
            return Path.Combine(pc?.RootPath ?? RootDir, AssetPathUtils.GetSubpath<T>(assetSubpath, context)) + (assetSubpath == null ? "" : FileExtensionWithDot); ;
        }
        public string GetPath(object obj, string assetSubpath = null, object context = null)
        {
            var pc = context.ObjectAsType<PersistenceContext>();
            return Path.Combine(pc?.RootPath ?? RootDir, AssetPathUtils.GetSubpath(obj, assetSubpath, context)) + (assetSubpath == null ? "" : FileExtensionWithDot); ;
        }

        public abstract string FileExtension { get; }
        public string FileExtensionWithDot { get { return (string.IsNullOrWhiteSpace(FileExtension) ? "" : "." + FileExtension); } }


        public async Task<IEnumerable<string>> Find<T>(string searchString = null, object context = null)
        {
            return await Task.Run(() =>
            {
                var dir = GetPath<T>(context: context);
                if (searchString == null) searchString = "*";
                var result = new List<string>();
                foreach (var path in Directory.GetFiles(dir, searchString + FileExtensionWithDot))
                {
                    var assetName = path.Replace(dir, "").TrimStart('/').TrimStart('\\').Replace(FileExtensionWithDot, "");
                    result.Add(assetName);
                }
                return (IEnumerable<string>)result;
            });
        }
    }
}
