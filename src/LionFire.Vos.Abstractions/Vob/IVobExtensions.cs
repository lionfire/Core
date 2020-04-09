using LionFire.Persistence;
using LionFire.Referencing;
using System;
using System.Collections.Generic;

namespace LionFire.Vos
{
    public static class IVobExtensions
    {

        public static bool IsRoot(this IVob vob) => ReferenceEquals(vob.Root, vob);

        public static T GetOwnRequired<T>(this IVob vob) where T : class => vob.AcquireOwn<T>() ?? throw new NotFoundException($"{typeof(T).FullName} not found on Vob: {vob}");
        public static T GetNextRequired<T>(this IVob vob, bool skipOwn = false) where T : class 
            => vob.AcquireNext<T>() ?? throw new NotFoundException($"{typeof(T).FullName} not found on Vob {vob} {(skipOwn ? "" : "or its")} ancestors");


        public static IVob QueryChild(this IVob vob, params string[] subpathChunks) => vob.QueryChild(subpathChunks, 0);
        public static IVob GetChild(this IVob vob, IEnumerable<string> subpathChunks) => vob.GetChild(subpathChunks.GetEnumerator());

        public static IVob GetRelativeOrAbsolutePath(this IVob vob, string path)
            => path == null ? null : (path.StartsWith(LionPath.Separator) ? vob.Root[path] : vob[path]);

        public static IVob GetOrQueryChild(this IVob vob, string subPath, bool createIfMissing = false)
            => createIfMissing ? vob[subPath] : vob.QueryChild(LionPath.ToPathArray(subPath));

        public static IReadHandle GetReadHandle(this IVob vob, Type T)
            => (IReadHandle)typeof(IVob).GetMethod("GetReadHandle").MakeGenericMethod(T).Invoke(vob, null);
        public static IReadWriteHandle GetReadWriteHandle(this IVob vob, Type T)
            => (IReadWriteHandle)typeof(IVob).GetMethod("GetReadWriteHandle").MakeGenericMethod(T).Invoke(vob, null);
        public static IWriteHandle GetWriteHandle(this IVob vob, Type T)
            => (IWriteHandle)typeof(IVob).GetMethod("GetWriteHandle").MakeGenericMethod(T).Invoke(vob, null);
    }
}
