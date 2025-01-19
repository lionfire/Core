using LionFire.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.IO.Reactive;

public class ObservableFileInfos
{

#if UNUSED // Specifying/obtaining a SourceCache - bad idea to modify the pristine read state (use another SourceCache for tracking pending writes)
    public static IObservable<IChangeSet<FileInfo, string>> PollOnDemand(string dir, out SourceCache<FileInfo, string> sourceCache)
    {
        sourceCache = new SourceCache<FileInfo, string>(x => x.Name);
        return PollOnDemand(sourceCache, dir);
    }

    public static IObservable<IChangeSet<FileInfo, string>> PollOnDemand(SourceCache<FileInfo, string> sourceCache, string dir, string? searchPattern = null)
    {
        // ENH: Parameter for FileInfoDidNotChange

        ArgumentNullException.ThrowIfNull(sourceCache);
        return sourceCache.ConnectOnDemand(resourceFactory: resourceFactory(dir, searchPattern));
    }

    //public static IObservable<IChangeSet<FileInfo, string>> PollOnDemand(string dir, string? searchPattern = null)
    //{
    //    var sourceCache = new SourceCache<FileInfo, string>(x => x.Name);
    //    return PollOnDemand(sourceCache, dir, searchPattern);
    //}
#endif

    public static IObservable<IChangeSet<FileInfo, string>> PollOnDemand(string dir, string? searchPattern = null)
    {
        return ObservableEx2.CreateConnectOnDemand(x => x.Name,
            resourceFactory: resourceFactory(dir, searchPattern)
        );
    }

    #region Equality

    /// <summary>
    /// Does not do a full equality check, just checks if the file length and last write time are the same.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool FileInfoDidNotChange(FileInfo a, FileInfo b)
    {
        if (a.Length != b.Length) return false;
        if (a.LastWriteTime != b.LastWriteTime) return false;

        return true;
    }

    #endregion

    #region (private)

    private static Func<SourceCache<FileInfo, string>, IDisposable> resourceFactory(string dir, string? searchPattern)
    {
        return cache =>
        {
            Debug.WriteLine("Started polling");
            bool running = true;
            Task.Run(DoPoll(dir, searchPattern, cache, () => running));
            return Disposable.Create(() =>
            {
                running = false;
                Debug.WriteLine("Stopped polling");
            });
        };
    }

    private static Func<Task?> DoPoll(string dir, string? searchPattern, SourceCache<FileInfo, string> cache, Func<bool> running)
    {
        return async () =>
        {
            while (running())
            {
                var items = (searchPattern != null
                    ? Directory.GetFiles(dir, searchPattern)
                    : Directory.GetFiles(dir)
                    ).ToList().Select(file => new FileInfo(file));
                cache.EditDiff(items, (a, b) => FileInfoDidNotChange(a, b));
                await Task.Delay(1000);
            }
        };
    }

    #endregion

#if false // Dislike
    public static IObservableCache<FileInfo, string> CreateOnDemandCache(string dir)
    {
        var sourceCache = new SourceCache<FileInfo, string>(x => x.Name);

        var cache = sourceCache.OnDemand(
            onStart: cache =>
            {
                Debug.WriteLine("Started polling");
                bool running = true;
                Task.Run(async () =>
                {
                    while (running)
                    {
                        var items = Directory.GetFiles(dir).ToList().Select(file => new FileInfo(file));
                        cache.EditDiff(items, (a, b) => FileInfoDidNotChange(a, b));
                        await Task.Delay(1000);
                    }
                });
                return Disposable.Create(() =>
                {
                    running = false;
                    Debug.WriteLine("Stopped polling");
                });
            }
        );

        return cache;
    }
#endif
}

