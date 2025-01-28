using LionFire.ExtensionMethods;
using LionFire.Reactive.Persistence;
using Microsoft.Extensions.Options;
using ReactiveUI;
using System.IO;
using System.Reactive.Linq;

namespace LionFire.IO.Reactive.Hjson;

public class HjsonFsDirectoryWriterRx<TKey, TValue> : IObservableWriter<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    public DirectorySelector Dir { get; }

    #region Lifecycle

    // ENH: Use FS Resilience pipeline
    public HjsonFsDirectoryWriterRx(DirectorySelector dir)
    {
        Dir = dir;
    }

    #endregion

    #region State

    private bool directoryExists;

    #endregion

    public IDisposable Synchronize(IObservable<TValue> source, TKey key, WritingOptions? options = null)
    {
        var filePath = GetFilePath(key);
        return source
            .Throttle(options?.DebounceDelay ?? TimeSpan.FromSeconds(1))
            .Subscribe(async value => await WriteToFile(filePath, value));
    }

    public IDisposable Synchronize(IReactiveNotifyPropertyChanged<IReactiveObject> source, TKey key, WritingOptions? options = null)
    {
        var filePath = GetFilePath(key);
        return source.Changed
            .Throttle(options?.DebounceDelay ?? TimeSpan.FromSeconds(1))
            .Subscribe(async _ => await WriteToFile(filePath, (TValue)source));
    }

    public ValueTask Write(TKey key, TValue value)
    {
        var filePath = GetFilePath(key);
        return WriteToFile(filePath, value);
    }
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

    private string GetFilePath(TKey key)
    {
        return Path.Combine(Dir.Path, $"{key}.hjson");
    }

    void CreateDirIfMissing()
    {
        if (directoryExists || Dir.Path == null) return;
        if (!Directory.Exists(Dir.Path))
        {
            Directory.CreateDirectory(Dir.Path);
        }
        directoryExists = true;
    }

    private async ValueTask WriteToFile(string filePath, TValue value)
    {
        CreateDirIfMissing();
        var bytes = HjsonSerialization.Serialize(value);
        await File.WriteAllBytesAsync(filePath, bytes).ConfigureAwait(false);
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

