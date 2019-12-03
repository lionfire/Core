// REVIEW Consider moving to LionFire.Referencing.dll
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Referencing
{
    public static class LionPath
    {

        // REFACTOR: Use ReferenceConstants.PathSeparator
        public const string Separator = "/";
        // REFACTOR: Use ReferenceConstants.PathSeparatorChar
        public const char SeparatorChar = '/';

        #region Delimiters

        // REFACTOR: Use ReferenceConstants or VReferenceConstants
        public const string HostDelimiter = "//"; // Use @?
        public const char PortDelimiter = ':';
        public const char PathDelimiter = '/'; // Redundant to separator?
        public const char PathDelimiterAlt = '\\'; // Redundant to separator?
        public static char[] Delimiters { get { return new char[] { '/', ':', '@', '|', '\\' }; } }

        #endregion
        
        // Field arrays can't be made readonly
        public static char[] SeparatorChars { get { return new char[] { PathDelimiter, PathDelimiterAlt }; } }

        public static string Combine(string path1, string path2)
        {
            if (string.IsNullOrWhiteSpace(path1)) return path2;
            if (string.IsNullOrWhiteSpace(path2)) return path1;

            return String.Concat(path1.TrimEnd(SeparatorChar), SeparatorChar, path2.TrimStart(SeparatorChar));
        }
        public static IEnumerable<string> PathSeparatorRepeater
        {
            get { yield return LionPath.Separator; }
        }
        public static string Combine(params string[] paths)
        {
            // UNTESTED
            if (paths.Length == 0) return String.Empty;

            return (paths[0].StartsWith(Separator) ? Separator : "") + String.Concat(paths.Zip(PathSeparatorRepeater, (x, y) => x.Trim(SeparatorChar) + y)).TrimEnd(SeparatorChar);
        }

        public static string GetDirectoryName(string path)
        {
            return System.IO.Path.GetDirectoryName(path).Replace('\\', '/'); // TODO
        }
        public static string GetFileName(string path)
        {
            return System.IO.Path.GetFileName(path); // TODO
        }
        public static string GetExtension(string path)
        {
            return System.IO.Path.GetExtension(path); // TODO
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
            if (path == null) return new string[] { };
            return path.Split(LionPath.SeparatorChars, StringSplitOptions.RemoveEmptyEntries);
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

        public static string CleanAbsolutePathEnds(string p)
        {
            return String.Concat(SeparatorChar, p.TrimStart(SeparatorChars).TrimEnd(SeparatorChars));
        }

        public static int GetAbsolutePathDepth(string path)
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
            int lastIndex = path.LastIndexOf(LionPath.PathDelimiter);
            lastIndex = Math.Max(path.LastIndexOf(LionPath.PathDelimiterAlt), lastIndex);
            if (lastIndex == -1) return path;

            return path.Substring(lastIndex + 1);
        }

        public static bool IsAbsolute(string arg)
        {
            return arg.StartsWith(PathDelimiter.ToString());
        }
        public static bool IsRelative(string arg) { return !IsAbsolute(arg); }
    }
}
