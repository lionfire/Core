using LionFire.Referencing;
using System;
using System.Collections;
using System.IO;

namespace LionFire.Vos
{

    public static class VosPath
    {

        #region Conventions

        public const string CachePrefix = "_#"; // FUTURE: Make this a configurable convention
        public const char LayerDelimiter = '|';
        public const string LayerDelimiterString = "|";
        public const char LocationDelimiter = '^';
        public const string LocationDelimiterString = "^";

        #endregion        

        public static string GetRoot(string rootName = null)
            => "/" + (rootName == null ? "" : "../" + rootName);
        public const string Root = "/";

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

        #region MachineSubpath

        public static string MachineSubpath { get { return "/Machine/" + LionFireEnvironment.MachineName + "/"; } }
        public static string MachineSubpathWithSeparators { get { return "/Machine/" + LionFireEnvironment.MachineName + "/"; } }

        #endregion



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

        public static string GetTypeNameFromPath(string path)
        {
            int index = path.IndexOf(VosPath.TypeDelimiter);
            if (index == -1) return null;
            return path.Substring(index, path.IndexOf(VosPath.TypeEndDelimiter, index) - index);
        }

        public static string GetRootOfPath(string path)
        {
            if(path.StartsWith("/../"))
            {
                var split = path.Split(new char[] { LionPath.SeparatorChar }, 3, StringSplitOptions.RemoveEmptyEntries);
                if(split.Length >= 3)
                {
                    return "/../" + split[1];
                }
            }
            return "/";
        }
    }
}
