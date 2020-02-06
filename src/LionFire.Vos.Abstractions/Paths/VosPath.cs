using System;
using System.Collections;

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
    }
}
