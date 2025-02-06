using LionFire.IO.Reactive.Filesystem;
using LionFire.Reactive.Persistence;
using System.IO;
using System.Text.RegularExpressions;

namespace LionFire.IO.Reactive.Hjson;
 
file static class _StringX
{

    public static string ToKebabCase(this string input)
    {
        // Replace spaces with hyphens and convert to lowercase
        return Regex.Replace(input, @"\s+", "-").ToLower();
    }
}

public class HjsonFsDirectoryReaderRx<TKey, TValue> : FsDirectoryReaderRx<TKey, TValue>, IObservableReader<TKey, TValue>
where TKey : notnull
where TValue : notnull
{
    protected override string Extension => ".hjson";
 
    #region Lifecycle

    public HjsonFsDirectoryReaderRx(DirectorySelector dir) : base(dir)
    {
        _ = LoadKeys();
    }

    #endregion

    public override IObservable<TValue?> Listen(TKey key)
    {
        var filePath = GetFilePath(key);
        return Observable.Create<TValue?>(observer =>
        {
            if (File.Exists(filePath))
            {
                var value = ReadFromFile(filePath);
                observer.OnNext(value);
            }

            var watcher = new FileSystemWatcher(Path.GetDirectoryName(filePath)!, Path.GetFileName(filePath))
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
            };

            FileSystemEventHandler onChanged = (s, e) =>
            {
                if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created)
                {
                    var value = ReadFromFile(filePath);
                    observer.OnNext(value);
                }
            };

            watcher.Changed += onChanged;
            watcher.Created += onChanged;
            watcher.EnableRaisingEvents = true;

            return () =>
            {
                watcher.Changed -= onChanged;
                watcher.Created -= onChanged;
                watcher.Dispose();
            };
        });
    }
    protected override TValue? ReadFromFile(string filePath)
    {
        var bytes = File.ReadAllBytes(filePath);
        return HjsonSerialization.Deserialize<TValue>(bytes);
    }
}

