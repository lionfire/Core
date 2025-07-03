using DynamicData.Kernel;
using LionFire.Dependencies;
using LionFire.ExtensionMethods.Copying;
using LionFire.ExtensionMethods.Poco.Getters;
using LionFire.IO.Reactive.Filesystem;
using LionFire.Persistence.Filesystemlike;
using LionFire.Persistence.Persisters;
using LionFire.Reactive.Persistence;
using LionFire.Vos.Schemas;
using Microsoft.Extensions.Logging;
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

    public HjsonFsDirectoryReaderRx(DirectorySelector dir, DirectoryTypeOptions directoryTypeOptions, ResiliencePipelineProvider<string> resiliencePipelineProvider, ILogger<HjsonFsDirectoryReaderRx<TKey, TValue>> logger) : base(dir, directoryTypeOptions, logger, deferInit: true)
    {
        ResiliencePipelineProvider = resiliencePipelineProvider ?? throw new ArgumentNullException();
        retryOnFileChange_Slow = ResiliencePipelineProvider.GetPipeline(FilesystemRetryPolicy.OnFileChange.Slow) ?? throw new ArgumentNullException();

        _ = Initialize();
    }

    #endregion

    protected override IObservable<TValue?> CreateValueObservable(TKey key)
    {
        Logger?.LogInformation("{0} CreateValueObservable for key {1}", Dir.Path, key);

        var schemaFlags = VosSchema.Flags<TValue>();

        return Observable.Create<TValue?>(observer =>
        {
            var keyedFilePath = GetKeyedFilePath(key);
            var defaultFilePath = GetDefaultFilePath(key);

            string filePath = schemaFlags.HasFlag(VosFlags.PreferDirectory) ? defaultFilePath : keyedFilePath;

            if (!File.Exists(filePath))
            {
                var otherPath = schemaFlags.HasFlag(VosFlags.PreferDirectory) ? keyedFilePath : defaultFilePath;
                if (File.Exists(otherPath))
                {
                    filePath = otherPath;
                }
            }

            var watcher = new FileSystemWatcher(Path.GetDirectoryName(filePath) ?? "", Path.GetFileName(filePath) ?? throw new ArgumentNullException("key => " + nameof(filePath)))
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
            };

            FileSystemEventHandler onChangedOrCreated = async (s, e) =>
            {
                //if (e.ChangeType == WatcherChangeTypes.Changed
                //|| e.ChangeType == WatcherChangeTypes.Created
                //)
                //{
                try
                {
                    var sw = Stopwatch.StartNew();

                    var existingValue = this.ObservableCache.Lookup(key);

                    var value = await ReadFromFile(filePath);
                    Logger?.LogInformation("Listening to {0}  ...loaded changes in {1}ms", filePath, sw.ElapsedMilliseconds);

                    if (existingValue.HasValue && value.HasValue)
                    {
                        existingValue.Value.AssignFrom(value.Value, Copying.AssignmentMode.Assign);
                        value = existingValue.Value;
                    }

                    observer.OnNext(value.HasValue ? value.Value : default);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
                //}
            };
            FileSystemEventHandler onDeleted = (s, e) =>
            {
                observer.OnNext(default);
            };

            watcher.Changed += onChangedOrCreated;
            watcher.Created += onChangedOrCreated;
            watcher.Deleted += onDeleted;
            watcher.EnableRaisingEvents = true;

            // Initial load
            _ = Task.Run(async () =>
            {

                if (File.Exists(filePath))
                {
                    Logger?.LogInformation($"Listening to {filePath}");
                    try
                    {
                        var sw = Stopwatch.StartNew();
                        var value = await ReadFromFile(filePath);
                        Logger?.LogInformation($"Listening to {filePath}  ...loaded in {sw.ElapsedMilliseconds}ms");
                        observer.OnNext(value.HasValue ? value.Value : default);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        //return Disposable.Empty; // Early exit on error
                    }
                }
                else
                {
                    Logger?.LogInformation($"Listening to {filePath}, but it does not exist");
                }
            });

            return Disposable.Create(() =>
            {
                Logger?.LogInformation($"Stopping listening to {filePath}");

                watcher.EnableRaisingEvents = false;
                watcher.Changed -= onChangedOrCreated;
                watcher.Created -= onChangedOrCreated;
                watcher.Deleted -= onDeleted;
                watcher.Dispose();
            });
        });
    }
    protected override async ValueTask<Optional<TValue>> ReadFromFile(string filePath)
    {
        if (retryOnFileChange_Slow == null)
        {
            throw new ObjectDisposedException(""); // REVIEW - how can this happen?
        }

        var bytes = await retryOnFileChange_Slow.ExecuteAsync(async context =>
        {
            Logger?.LogInformation("Reading " + filePath);
            // TODO: File Exists check only after ReadAllBytesAsync fails (with what exception?)

            if (!File.Exists(filePath)) return null; // TODO Async
            return await File.ReadAllBytesAsync(filePath);
        }).ConfigureAwait(false);

        if (bytes == null) return Optional<TValue>.None;

        return HjsonSerialization.Deserialize<TValue>(bytes);
    }
}

