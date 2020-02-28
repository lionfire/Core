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

        public static string GetAncestor(string path, int depth, bool nullIfBeyondRoot = false)
        {
            bool isAbsolute = IsAbsolute(path);
            while (depth > 0)
            {
                var lastIndex = path.LastIndexOf(LionPath.SeparatorChar);

                if (lastIndex < 0)
                {
                    if (nullIfBeyondRoot) return null;
                    else return isAbsolute ? LionPath.Separator : "";
                }
                path = path.Substring(0, lastIndex);
            }
            return path;
        }

        public const char PathDelimiter = '/'; // Redundant to separator?
        public const char PathDelimiterAlt = '\\'; // Redundant to separator?
        public static char[] Delimiters { get { return new char[] { '/', ':', '@', '|', '\\' }; } }

        #endregion

        public static string FromPathArray(IEnumerable<string> chunks)
        {
            var sb = new StringBuilder();
            foreach (var chunk in chunks)
            {
                sb.Append(Separator);
                sb.Append(chunk);
            }
            if (sb.Length == 0) sb.Append(Separator);
            return sb.ToString();
        }
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

        private const bool preserveEndSeparator = true;

        public static string Normalize(string path) => path?.Replace(@"\", "/");

        public static string Combine(params string[] paths) => Combine((IEnumerable<string>)paths);

        public static string Combine(IEnumerable<string> paths)
        {
            // UNTESTED
            if (!paths.Any()) return String.Empty;

            var sb = new StringBuilder();

            if (SeparatorChars.Contains(paths.First()[0])) sb.Append(Separator);

            bool isFirst = true;
            foreach (var chunk in paths)
            {
                if (isFirst) isFirst = false;
                else sb.Append(Separator);
                sb.Append(chunk.Trim(SeparatorChars));
            }

            if (preserveEndSeparator)
            {
                var lastChunk = paths.Last();
                var lastChar = lastChunk.Last();
                if (SeparatorChars.Contains(lastChar))
                {
                    sb.Append(Separator);
                }
            }

            return sb.ToString();
            //return (paths[0].StartsWith(Separator) ? Separator : "") + String.Concat(PathSeparatorRepeater.Zip(paths, (separator, chunk) => x.Trim(SeparatorChar) + y)).TrimEnd(SeparatorChar);
        }

        public static string GetDirectoryName(string path) => System.IO.Path.GetDirectoryName(StripSpecifiers(path)).Replace('\\', '/');
        public static string GetFileName(string path) => System.IO.Path.GetFileName(StripSpecifiers(path));

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

        public static string GetTrimmedAbsolutePath(string p)
        {
            // TODO: Remove superfluous separators inside path?
            // TODO: Replace non-primary separators in path?
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

        public static bool IsAbsolute(string arg) => arg.StartsWith(PathDelimiter.ToString());
        public static bool IsRelative(string arg) { return !IsAbsolute(arg); }

        public static bool IsSameOrDescendantOf(string parentPath, string childPath)
        {
            var parentChunks = LionPath.ToPathArray(parentPath);
            var childChunks = LionPath.ToPathArray(childPath);
            if (childChunks.Length < parentChunks.Length) return false;
            int i = 0;
            foreach (var parentChunk in parentChunks)
            {
                if (childChunks[i++] != parentChunk) return false;
            }
            return true;
        }
        public static bool IsDescendantOf(string parentPath, string childPath)
        {
            var parentChunks = LionPath.ToPathArray(parentPath);
            var childChunks = LionPath.ToPathArray(childPath);
            if (childChunks.Length < parentChunks.Length) return false;
            int i = 0;
            foreach (var parentChunk in parentChunks)
            {
                if (childChunks[i++] != parentChunk) return false;
            }
            return childChunks.Length > parentChunks.Length;
        }


        /// <summary>
        /// Indicates that if there is an extension (due to an auto-extension feature)), it hasn't appeared yet in the string
        /// </summary>
        public static string ExplicitNoExtensionSuffix { get; set; } = ":...";

        /// <summary>
        /// Indicates that this suffix should be stripped, and then whatever appears after a . (if exists) should be treated as an extension.  If there is no ., the extension is null.
        /// </summary>
        public static string ExplicitHasExtension { get; set; } = ":.";

        public static string StripSpecifiers(string lionPath) => lionPath.TrimEnd(ExplicitNoExtensionSuffix).TrimEnd(ExplicitHasExtension);

        public static string GetExtension(string path)
        {
            if (path.EndsWith(ExplicitNoExtensionSuffix)) return null;
            if (path.EndsWith(ExplicitHasExtension)) path = path.Substring(0, path.Length - ExplicitHasExtension.Length);

            var result = System.IO.Path.GetExtension(path);
            return result.Length == 0 ? null : result;
        }
    }

    public static class LionPathStringExtensions
    {
        public static string TrimEnd(this string str, string trimString)
        {
            if (str.EndsWith(trimString)) return str.Substring(0, str.Length - trimString.Length);
            return str;
        }
    }
}
