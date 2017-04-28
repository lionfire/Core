using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    public static class VosPath
    {

        public const string Separator = "/";
        public const char SeparatorChar = '/';

        #region Delimiters

        public const string HostDelimiter = "//"; // Use @?
        public const char LayerDelimiter = '|';
        public const char LocationDelimiter = '^';
        public const char PortDelimiter = ':';
        public const char PathDelimiter = '/'; // Redundant to separator?
        public const char PathDelimiterAlt = '\\'; // Redundant to separator?
        public static char[] Delimiters { get { return new char[] { '/', ':', '@', '|', '\\' }; } }


        #endregion

        #region Conventions

        public const string CachePrefix = "_#"; // FUTURE: Make this a configurable convention

        #endregion

        // Field arrays can't be made readonly
        public static char[] SeparatorChars { get { return new char[] { PathDelimiter, PathDelimiterAlt }; } }

        public static string Combine(string path1, string path2)
        {
            if (StringX.IsNullOrWhiteSpace(path1)) return path2;
            if (StringX.IsNullOrWhiteSpace(path2)) return path1;

            return String.Concat(path1.TrimEnd(SeparatorChar), SeparatorChar, path2.TrimStart(SeparatorChar));
        }
        public static IEnumerable<string> PathSeparatorRepeater
        {
            get { yield return VosPath.Separator; }
        }
        public static string Combine(params string[] paths)
        {
            // UNTESTED
            if (paths.Length == 0) return String.Empty;

            return (paths[0].StartsWith(Separator)  ? Separator : "") + String.Concat(paths.Zip(PathSeparatorRepeater,(x,y) => x.Trim(SeparatorChar) + y)).TrimEnd(SeparatorChar);
        }

        public static string Combine(string path1, IEnumerable<string> path2)
        {
            var sb = new StringBuilder();
            sb.Append(path1);
            foreach (var path in path2)
            {
                sb.Append(PathDelimiter);
                sb.Append(path);
            }
            return sb.ToString();
        }

        public static string[] ToPathArray(this string path)
        {
            if(path==null)return new string[]{};
            return path.Split( VosPath.SeparatorChars, StringSplitOptions.RemoveEmptyEntries);
        }

        //public static string[] ToPathArray(this string path)
        //{
        //    return path.Split(new char[] { VosPath.SeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
        //}

        //public static string[] GetChunks(string subpath)
        //{
        //    return subpath.Split(SeparatorChars, StringSplitOptions.RemoveEmptyEntries);
        //}

        //public static string[] ToPathArray(params string[] subpaths)
        //{
        //    // OPTIMIZE?
        //    List<string> chunks = new List<string>();
        //    foreach (var subPathItem in subpaths)
        //    {
        //        chunks.AddRange(subPathItem.Split(SeparatorChar));
        //    }
        //    return chunks.ToArray();
        //}

        internal static string CleanAbsolutePathEnds(string p)
        {
            return String.Concat(SeparatorChar, p.TrimStart(SeparatorChars).TrimEnd(SeparatorChars));
        }

        internal static int GetAbsolutePathDepth(string path)
        {
            // TODO: Debug-time SanityCheck for is absolute path
            return path.Count(c => c == SeparatorChar);
        }

        /// <summary>
        /// Returns the substring after the last VosPath.PathDelimiter.  If path is null, returns null.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetName(string path)
        {
            if (path == null) return null;
            int lastIndex = path.LastIndexOf(VosPath.PathDelimiter);
            lastIndex = Math.Max(path.LastIndexOf(VosPath.PathDelimiterAlt), lastIndex);
            if (lastIndex == -1) return path;

            return path.Substring(lastIndex + 1);
        }


        #region Type Encoding 
        // MOvE this to a separate encoding class?

        public const bool AppendTypeNameToFileNames = false; // TEMP - TODO: Figure out a way to do this in VOS land

        public const char TypeDelimiter = '(';
        public const char TypeEndDelimiter = ')';

        public static string GetTypeNameFromFileName(string fileName)
        {
            int index = fileName.IndexOf(TypeDelimiter);
            if (index == -1) return null;
            return fileName.Substring(index, fileName.IndexOf(TypeEndDelimiter, index) - index);
        }

        //private static string GetDirNameForType(string filePath)
        //{
        //    var chunks = VosPath.ToPathArray(filePath);
        //    if (chunks == null || chunks.Length == 0) yield break;
        //    string parentDirName = chunks[chunks.Length - 1];

        //    return Assets.AssetPath.GetDefaultDirectory(typeof(T));
        //}

        #endregion

        #region Packages

        public const string PackagePrefix = "[";

        public static string PackageNameToStorageSubpath(string packageName)
        {
            return PackagePrefix + packageName + "]";
        }

        #endregion

        #region Children

        public static bool IsHidden(string childName)
        {
            return childName.StartsWith(PackagePrefix);
        }

        #endregion

        #region VobHandle

        // DUPLICATE See also VosAssets.AssetPathToVobHandle
        // SIMILAR - REFACTOR See also VosContext resolver?

#if AOT
		public static IVobHandle PathToVobHandle(this string path, string package=null, string
		                                         store=null, Type T=null)
#else
		public static VobHandle<T> PathToVobHandle<T>(this string path, string package = null, string
store = null)
where T : class
#endif
        {
#if AOT
			if(T==null)throw new ArgumentNullException("T");
#endif
            Vob vob;

            var context = VosContext.Current;

            if (context != null)
            {
                if (package == null) package = context.Package;
                if (store == null) store = context.Store;
            }

            if (package != null)
            {
                if (store == null)
                {
                    vob = VosApp.Instance.Packages[package][path];
                }
                else
                {
                    vob = VosApp.Instance.PackageStores[package][store][path];
                    //vob = VosApp.Instance.Archives[location][VosPath.PackageNameToStorageSubpath(package)][path];
                }
            }
            else // package == null
            {
                if (store == null)
                {
                    vob = V.Root[path];
                }
                else
                {
                    //l.Trace("Location only, no package? - prob fine but not planned for existing apps");
                    vob = VosApp.Instance.Stores[store][path];
                }
            }
#if AOT
            return vob.ToHandle(T);
#else
            return vob.ToHandle<T>();
#endif
        }

        #endregion

        #region Misc

        private static ILogger l = Log.Get();
		
        #endregion

        public static bool IsAbsolute(string arg)
        {
            return arg.StartsWith(PathDelimiter.ToString());
        }
        public static bool IsRelative(string arg) { return !IsAbsolute(arg); }


        #region MachineSubpath

        public static string MachineSubpath { get { return "/Machine/" + LionEnvironment.MachineName + "/"; } }
        public static string MachineSubpathWithSeparators { get { return "/Machine/" + LionEnvironment.MachineName + "/"; } }

        #endregion


    }

    public static class VosPathExtensions
    {
        //public static string[] ToPathArray(this string[] subpaths)
        //{
        //    // OPTIMIZE?
        //    List<string> chunks = new List<string>();
        //    foreach (var subPathItem in subpaths)
        //    {
        //        chunks.AddRange(subPathItem.Split(new char[] { VosPath.SeparatorChar }, StringSplitOptions.RemoveEmptyEntries));
        //    }
        //    return chunks.ToArray();
        //}

        public static IEnumerable<string> ToPathElements(this string path)
        {
            foreach (var chunk in VosPath.ToPathArray(path))
            {
                yield return chunk;
            }
        }

        public static IEnumerable<string> ToPathElements(this string[] subpaths)
        {
            foreach (var subPathItem in subpaths)
            {
                foreach (var chunk in VosPath.ToPathArray(subPathItem))
                {
                    yield return chunk;
                }
            }
        }

        public static string ToSubPath(this IEnumerable<string> relativePath)
        {
            if (relativePath == null) return null;
            if (!relativePath.Any()) return null;
            return relativePath.Aggregate((x, y) => x + VosPath.Separator + y);
        }

        public static string ToSubPath(this IEnumerator
#if !AOT
            <string>
#endif
            relativePath)
        {
            if (relativePath == null) return null;

            List<string> pathChunks = new List<string>();

            while (relativePath.MoveNext())
            {
                pathChunks.Add(relativePath.Current
#if AOT
                    as string
#endif
                    );
            }
            return pathChunks.ToSubPath(); // OPTIMIZE
        }

        public static string ToPath(this IEnumerable<string> absolutePath)
        {
            if (absolutePath == null) return null;
            if (!absolutePath.Any()) return null;
            return String.Concat(VosPath.Separator, absolutePath.Aggregate((x, y) => x + VosPath.Separator + y));
        }
    }
}
