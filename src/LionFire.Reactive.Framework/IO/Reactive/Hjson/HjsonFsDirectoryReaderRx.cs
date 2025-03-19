using DynamicData.Kernel;
using LionFire.ExtensionMethods.Poco.Getters;
using LionFire.IO.Reactive.Filesystem;
using LionFire.Persistence.Filesystemlike;
using LionFire.Persistence.Persisters;
using LionFire.Reactive.Persistence;
using Microsoft.Extensions.Resilience;
using Polly;
using Polly.Registry;
using System.Diagnostics;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;

namespace LionFire.IO.Reactive.Hjson;

public class HjsonFsDirectoryReaderRx<TKey, TValue> : FsDirectoryReaderRx<TKey, TValue>, IObservableReader<TKey, TValue>
where TKey : notnull
where TValue : notnull
{
    #region Constants

    protected override string Extension => ".hjson";

    #endregion

    #region Dependencies
    ResiliencePipelineProvider<string> ResiliencePipelineProvider { get; }
    ResiliencePipeline retryOnFileChange_Slow;

    #endregion  

    #region Lifecycle

    public HjsonFsDirectoryReaderRx(DirectorySelector dir, DirectoryTypeOptions directoryTypeOptions, ResiliencePipelineProvider<string> resiliencePipelineProvider) : base(dir, directoryTypeOptions)
    {
        ResiliencePipelineProvider = resiliencePipelineProvider ?? throw new ArgumentNullException();
        retryOnFileChange_Slow = ResiliencePipelineProvider.GetPipeline(FilesystemRetryPolicy.OnFileChange.Slow) ?? throw new ArgumentNullException();

    }

    #endregion

    protected override IObservable<TValue?> CreateValueObservable(TKey key)
    {
        var filePath = GetFilePath(key);

        return Observable.Create<TValue?>(observer =>
        {
            Console.WriteLine("Listening to " + filePath);
            _ = Task.Run(async () =>
            {

                if (File.Exists(filePath))
                {
                    try
                    {
                        var value = await ReadFromFile(filePath);
                        observer.OnNext(value.HasValue ? value.Value : default);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        //return Disposable.Empty; // Early exit on error
                    }
                }
            });

            var watcher = new FileSystemWatcher(Path.GetDirectoryName(filePath) ?? "", Path.GetFileName(filePath) ?? throw new ArgumentNullException("key => " + nameof(filePath)))
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
            };

            FileSystemEventHandler onChanged = async (s, e) =>
            {
                if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Deleted)
                {
                    try
                    {
                        var value = await ReadFromFile(filePath);
                        observer.OnNext(value.HasValue ? value.Value : default);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                }
            };

            watcher.Changed += onChanged;
            watcher.Created += onChanged;
            watcher.Deleted += onChanged;
            watcher.EnableRaisingEvents = true;

            return Disposable.Create(() =>
            {
                Debug.WriteLine("Stopping listening to " + filePath);

                watcher.EnableRaisingEvents = false;
                watcher.Changed -= onChanged;
                watcher.Created -= onChanged;
                watcher.Deleted -= onChanged;
                watcher.Dispose();
            });
        });
    }
    protected override async ValueTask<Optional<TValue>> ReadFromFile(string filePath)
    {
        if(retryOnFileChange_Slow == null)
        {
            throw new ObjectDisposedException(""); // REVIEW - how can this happen?
        }

        var bytes = await retryOnFileChange_Slow.ExecuteAsync(async context =>
        {
            Debug.WriteLine("Reading " + filePath);
            // TODO: File Exists check only after ReadAllBytesAsync fails (with what exception?)

            if (!File.Exists(filePath)) return null; // TODO Async
            return await File.ReadAllBytesAsync(filePath);
        }).ConfigureAwait(false);

        if (bytes == null) return Optional<TValue>.None;

        return HjsonSerialization.Deserialize<TValue>(bytes);
    }
}

