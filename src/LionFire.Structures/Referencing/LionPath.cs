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


        private const bool preserveEndSeparator = true;


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

        #region Combining

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

        public static string Combine(IEnumerable<string> path1, string path2)
            => Combine(path1.Concat(new string[] { path2 }));
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

        public static string Combine(string path1, string path2)
        {
            if (string.IsNullOrWhiteSpace(path1)) return path2;
            if (string.IsNullOrWhiteSpace(path2)) return path1;

            return String.Concat(path1.TrimEnd(SeparatorChar), SeparatorChar, path2.TrimStart(SeparatorChar));
        }

        #endregion

        #region Conversion

        #region To Path

        public static string FromPathArray(IEnumerable<string> chunks, bool absolute = false)
        {
            var sb = new StringBuilder();
            if (absolute) { sb.Append(SeparatorChar); }
            foreach (var chunk in chunks)
            {
                sb.Append(Separator);
                sb.Append(chunk);
            }
            if (sb.Length == 0) sb.Append(Separator);
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunks"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="absolute">If true, return a path starting with /.  If false, don't.  If null, will be true iff startIndex == 0.</param>
        /// <returns></returns>
        public static string FromPathArray(string[] chunks, int startIndex = 0, int endIndex = -1, bool? absolute = null)
        {
            if (!absolute.HasValue) absolute = startIndex == 0;
            var sb = new StringBuilder();
            if (absolute.Value) { sb.Append(SeparatorChar); }

            if (endIndex < 0) { endIndex = chunks.Length + endIndex; }
            for (int i = startIndex; i < endIndex; i++)
            {
                if (i != startIndex) { sb.Append(Separator); }
                sb.Append(chunks[i]);
            }
            if (sb.Length == 0) sb.Append(Separator);
            return sb.ToString();
        }

        #endregion

        #region From Path

        public static string[] ToPathArray(this string path)
            => path == null
                ? Array.Empty<string>()
                : path.Split(SeparatorChars, StringSplitOptions.RemoveEmptyEntries);

        #endregion

        #endregion

        #region Inspection

        #region Parent Paths

        public static string GetParent(string path, bool nullIfBeyondRoot = false) => GetAncestor(path, 1, nullIfBeyondRoot);
        public static string GetAncestor(string path, int depth, bool nullIfBeyondRoot = false)
        {
            if (path == "/")
            {
                return depth == 0 ? "/" : (nullIfBeyondRoot ? null : "/");
            }

            bool endsWithSeparator = path.EndsWith(LionPath.PathDelimiter.ToString());
            path = path.TrimEnd(PathDelimiter);

            bool isAbsolute = IsAbsolute(path);
            for (; depth > 0; depth--)
            {
                var lastIndex = path.LastIndexOf(SeparatorChar);

                if (lastIndex < 0)
                {
                    if (nullIfBeyondRoot) return null;
                    else return isAbsolute ? Separator : "";
                }
                path = path.Substring(0, lastIndex);
            }

            if (endsWithSeparator)
            {
                path += PathDelimiter;
            }
            return path;
        }

        #endregion

        #region Subcomponents: Directory, Filename and Extension

        public static string GetDirectoryName(string path) => System.IO.Path.GetDirectoryName(StripSpecifiers(path)).Replace('\\', '/');
        public static string GetFileName(string path) => System.IO.Path.GetFileName(StripSpecifiers(path));

        // REDUNDANT to GetFileName???
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

        public static string GetExtension(string path)
        {
            if (path.EndsWith(ExplicitNoExtensionSuffix)) return null;
            if (path.EndsWith(ExplicitHasExtension)) path = path.Substring(0, path.Length - ExplicitHasExtension.Length);

            var result = System.IO.Path.GetExtension(path);
            if (result != null && result.Length == System.IO.Path.GetFileName(path).Length) return null; // Treat treat ".filename" as having no extension.
            return result.Length == 0 ? null : result;
        }

        #endregion

        #region Metrics

        public static int GetAbsolutePathDepth(string path)
        {
            // TODO: Debug-time SanityCheck for is absolute path
            return path.Count(c => c == SeparatorChar);
        }

        #endregion

        #region Absolute vs Relative

        public static bool IsAbsolute(string arg) => arg.StartsWith(PathDelimiter.ToString());
        public static bool IsRelative(string arg) { return !IsAbsolute(arg); }

        #endregion

        #region Ancestry queries

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

        #endregion

        #endregion

        #region Constant Conventions

        public static char[] SeparatorChars { get { return new char[] { PathDelimiter, PathDelimiterAlt }; } }
        public static IEnumerable<string> PathSeparatorRepeater { get { yield return Separator; } }


        /// <summary>
        /// Indicates that if there is an extension (due to an auto-extension feature)), it hasn't appeared yet in the string
        /// </summary>
        public static string ExplicitNoExtensionSuffix { get; set; } = ":...";

        /// <summary>
        /// Indicates that this suffix should be stripped, and then whatever appears after a . (if exists) should be treated as an extension.  If there is no ., the extension is null.
        /// </summary>
        public static string ExplicitHasExtension { get; set; } = ":.";

        #endregion

        #region Coercion / Normalization

        // OPTIMIZE?
        public static string Normalize(string path) => path?.Replace(@"\", "/");

        public static string StripSpecifiers(string lionPath) => lionPath.TrimEnd(ExplicitNoExtensionSuffix).TrimEnd(ExplicitHasExtension);

        public static string GetTrimmedAbsolutePath(string p)
        {
            // TODO: Remove superfluous separators inside path?
            // TODO: Replace non-primary separators in path?
            return String.Concat(SeparatorChar, p.TrimStart(SeparatorChars).TrimEnd(SeparatorChars));
        }

        #endregion

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
