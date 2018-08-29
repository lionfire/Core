using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Referencing
{
    // REVIEW

    public static class LionPathExtensions
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
            foreach (var chunk in LionPath.ToPathArray(path))
            {
                yield return chunk;
            }
        }

        public static IEnumerable<string> ToPathElements(this string[] subpaths)
        {
            foreach (var subPathItem in subpaths)
            {
                foreach (var chunk in LionPath.ToPathArray(subPathItem))
                {
                    yield return chunk;
                }
            }
        }

        public static string ToSubPath(this IEnumerable<string> relativePath)
        {
            if (relativePath == null) return null;
            if (!relativePath.Any()) return null;
            return relativePath.Aggregate((x, y) => x + LionPath.Separator + y);
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
            return String.Concat(LionPath.Separator, absolutePath.Aggregate((x, y) => x + LionPath.Separator + y));
        }
    }
}
