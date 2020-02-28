#nullable enable
//#define TRACE_SAVE
#define TRACE_LOAD

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LionFire.Dependencies;
using LionFire.Serialization;

namespace LionFire.Persistence
{
    public static class PersistenceOperationDeserializeExtensions
    {
        /// <summary>
        /// NOTE FIXME: currently specific to:
        ///  - file extensions, and
        ///  - multiple potential paths, and 
        ///  - streams
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="op"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        //public static async Task<T> ToObject<T>(this PersistenceOperation op)
        //    => await (op.Context?.SerializationProvider ?? DependencyLocator.TryGet<ISerializationProvider>()).ToObject<T>(op);

            

            // OLD
            //async IAsyncEnumerable<Stream> CandidateStreams(ISerializationStrategy strategy)
            //{
            //    foreach (var path in op.Deserialization.CandidatePaths)
            //    {
            //        if (!op.IgnoreFileExtension && !strategy.SupportsExtension(Path.GetExtension(path)))
            //        {
            //            continue;
            //        }

            //        yield return await context.Deserialization.PathToStream(path).ConfigureAwait(false);
            //    }
            //}




            //var strategyResults = .Strategies(streams(), op, context);

            //foreach (var strategyResult in strategyResults)
            //{
            //    var strategy = strategyResult.Strategy;
            //    foreach (var path in op.Deserialization.CandidatePaths)
            //    {
            //        if (!op.IgnoreFileExtension && !strategy.SupportsExtension(Path.GetExtension(path)))
            //        {
            //            continue;
            //        }

            //        //#if MONO
            //        //            fullDiskPath = fullDiskPath.Replace('\\', '/');
            //        //#else
            //        //                fullDiskPath = fullDiskPath.Replace('/', '\\'); // REVIEW
            //        //#endif

            //        using (var fs = context.Deserialization.PathToStream(path))
            //        {
            //            var Result) = strategy.ToObject<T>(fs, op, context);
            //            if (Result.IsSuccess)
            //            {
            //                return (Result.Object,Result);
            //            }
            //        }
            //    }
            //}
            //return (default(T), );

            //            using (var fs = new FileStream(diskPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            //            {
            //                if (firstRead)
            //                {
            //                    firstRead = false;
            //                }
            //                else
            //                {
            //                    fs.Seek(0, SeekOrigin.Begin);
            //                }

            //                var op = ((Func<PersistenceOperation>)(() => new SerializePersistenceOperation()
            //                {
            //                    Path = diskPath,
            //                    PathIsMissingExtension = null,
            //                })).ToLazy();


            //#if TRACE_LOAD
            //                l.Debug("[FS LOAD] " + diskPath);
            //#endif
            //                if (fs.Length == 0)
            //                {
            //                    l.Error("FsPersistence.GetFromPath found empty file. " + (AutoDeleteEmptyFiles ? "Autodeleting it." : "") + " - " + diskPath);
            //                    deleteFile = AutoDeleteEmptyFiles;
            //                    return null;
            //                }

            //                object obj = Deserialize(fs, type, diskPath, operation, context);
            //                return obj;
            //            }
    }
}
