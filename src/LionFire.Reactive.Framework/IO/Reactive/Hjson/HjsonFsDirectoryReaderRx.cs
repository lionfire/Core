﻿using LionFire.IO.Reactive.Filesystem;
using LionFire.Reactive.Persistence;
using System.IO;

namespace LionFire.IO.Reactive.Hjson;

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

}

