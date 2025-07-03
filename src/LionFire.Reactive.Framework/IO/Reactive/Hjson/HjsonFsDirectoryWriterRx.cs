using DynamicData.Kernel;
using LionFire.ExtensionMethods;
using LionFire.Reactive.Persistence;
using LionFire.Vos.Schemas;
using Microsoft.Extensions.Options;
using ReactiveUI;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace LionFire.IO.Reactive.Hjson;

public abstract class DirectoryWriterRx<TKey, TValue> : IDisposable
    //, IObservableWriter<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    #region (static) Configuration

    public static bool AutoCreateDirectories { get; set; } = true;

    #endregion

    protected static readonly VosFlags flags = VosSchema.Flags<TValue>();

    protected abstract IDirectoryAsync Directory { get; }

    #region Parameters

    public DirectorySelector Dir { get; }

    #region (Derived)

    public string EffectiveDir(TKey key)
    {
        return flags.HasFlag(VosFlags.PreferDirectory)
            ? Path.Combine(Dir.Path, key.ToString()!)
            : Dir.Path;
    }

    #endregion

    public DirectoryTypeOptions DirectoryTypeOptions { get; set; }

    #endregion

    #region Lifecycle

    public DirectoryWriterRx(DirectorySelector dir, DirectoryTypeOptions directoryTypeOptions)
    {
        Dir = dir;
        DirectoryTypeOptions = directoryTypeOptions ?? throw new ArgumentNullException(nameof(directoryTypeOptions));


        if (AutoCreateDirectories)
        {
            _ = CreateDirIfMissing();
        }
    }

    public virtual void Dispose()
    {
        writeOperations?.Dispose();
        writeOperations = null;
    }

    #endregion

    #region State

    /// <summary>
    /// Applies to the per-type directory, not the per-key directory, since the key isn't known.
    /// </summary>
    private bool directoryExists;

    #endregion

    #region Events

    public IObservable<WriteOperation<TKey, TValue>> WriteOperations => writeOperations ?? throw new ObjectDisposedException(null);

    private Subject<WriteOperation<TKey, TValue>>? writeOperations = new Subject<WriteOperation<TKey, TValue>>();

    #endregion

    #region Methods

    #region CreateDirIfMissing

    async protected ValueTask CreateDirIfMissing()
    {
        if (directoryExists || Dir.Path == null) return;
        var dir = Dir.Path;
        await _CreateDirIfMissing_Impl(dir);
    }

    async protected ValueTask CreateDirIfMissing(TKey key)
    {
        if (!flags.HasFlag(VosFlags.PreferDirectory))
        {
            await CreateDirIfMissing();
            return;
        }

        var dir = EffectiveDir(key);
        await _CreateDirIfMissing_Impl(dir);
    }

    async private ValueTask _CreateDirIfMissing_Impl(string dir)
    {
        await createDirSemaphore.WaitAsync();
        try
        {
            if (!await Directory.ExistsAsync(dir))
            {
                try
                {
                    await Directory.CreateDirectoryAsync(dir);
                    directoryExists = true;
                }
                catch
                {
                    directoryExists = false;
                    throw;
                }
            }
        }
        finally
        {
            createDirSemaphore.Release();
        }
    }
    private readonly SemaphoreSlim createDirSemaphore = new(1, 1);

    #endregion

    #endregion
}

public class HjsonFsDirectoryWriterRx<TKey, TValue>
    : FsDirectoryWriterRx<TKey, TValue>
      , IObservableWriter<TKey, TValue>
where TKey : notnull
where TValue : notnull
{
    #region Lifecycle

    // ENH: Use FS Resilience pipeline
    public HjsonFsDirectoryWriterRx(DirectorySelector dir, DirectoryTypeOptions directoryTypeOptions) : base(dir, directoryTypeOptions)
    {
    }

    #endregion



    public IDisposable Synchronize(IObservable<TValue> source, TKey key, WritingOptions? options = null)
    {
        var filePath = GetFilePath(key);
        return source
            .Throttle(options?.DebounceDelay ?? TimeSpan.FromSeconds(1))
            .Subscribe(async value => await WriteToFile(key, filePath, value));
    }

    public IDisposable Synchronize(IReactiveNotifyPropertyChanged<IReactiveObject> source, TKey key, WritingOptions? options = null)
    {
        var filePath = GetFilePath(key);
        return source.Changed
            .Throttle(options?.DebounceDelay ?? TimeSpan.FromSeconds(1))
            .Subscribe(async _ => await WriteToFile(key, filePath, (TValue)source));
    }

    #region Write

    public ValueTask Write(TKey key, TValue value)
    {
        var filePath = GetFilePath(key);
        return WriteToFile(key, filePath, value);
    }
    private async ValueTask WriteToFile(TKey key, string filePath, TValue value)
    {
        await CreateDirIfMissing(key);
        var bytes = HjsonSerialization.Serialize(value);
        await File.WriteAllBytesAsync(filePath, bytes).ConfigureAwait(false);
    }

    #endregion

    public async ValueTask<bool> Remove(TKey key)
    {
        var filePath = GetFilePath(key);

        return await Task.Run(() =>
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            else
            {
                return false;
            }
        });
    }

    const string HjsonExtension = ".hjson";


    private string GetFilePath(TKey key)
    {
        return flags.HasFlag(VosFlags.PreferDirectory)
            ? Path.Combine(EffectiveDir(key), $"{DirectoryTypeOptions.SecondExtension}{HjsonExtension}")
            : Path.Combine(EffectiveDir(key), $"{key}.{DirectoryTypeOptions.SecondExtension}{HjsonExtension}");
    }



}

public interface IDictionaryProvider
{
    IObservableReaderWriter<string, TValue> Get<TValue>()
        where TValue : notnull;
    IObservableReader<string, TValue> GetReadOnly<TValue>()
        where TValue : notnull;
    IObservableWriter<string, TValue> GetWriteOnly<TValue>()
        where TValue : notnull;
}

