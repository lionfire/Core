using DynamicData.Kernel;
using LionFire.IO.Reactive.Filesystem;
using LionFire.Persistence.Filesystemlike;
using LionFire.Reactive.Persistence;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
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

    public HjsonFsDirectoryReaderRx(DirectorySelector dir, IFileExtensionConvention extensionConvention) : base(dir, extensionConvention)
    {
    }

    #endregion
        
    protected override IObservable<TValue?> CreateValueObservable(TKey key)
    {
        var filePath = GetFilePath(key);

        return Observable.Create<TValue?>(observer =>
        {
            Console.WriteLine("Listening to " + filePath);
            if (File.Exists(filePath))
            {
                try
                {
                    var value = ReadFromFile(filePath);
                    observer.OnNext(value);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    return Disposable.Empty; // Early exit on error
                }
            }

            var watcher = new FileSystemWatcher(Path.GetDirectoryName(filePath) ?? "", Path.GetFileName(filePath) ?? throw new ArgumentNullException("key => " + nameof(filePath)))
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
            };

            FileSystemEventHandler onChanged = (s, e) =>
            {
                if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created)
                {
                    try
                    {
                        var value = ReadFromFile(filePath);
                        observer.OnNext(value);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                }
            };

            watcher.Changed += onChanged;
            watcher.Created += onChanged;
            watcher.EnableRaisingEvents = true;

            return Disposable.Create(() =>
            {
                Console.WriteLine("Stopping listening to " + filePath);

                watcher.EnableRaisingEvents = false;
                watcher.Changed -= onChanged;
                watcher.Created -= onChanged;
                watcher.Dispose();
            });
        });
    }
    protected override TValue? ReadFromFile(string filePath)
    {
        var bytes = File.ReadAllBytes(filePath);
        return HjsonSerialization.Deserialize<TValue>(bytes);
    }
}

