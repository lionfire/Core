using LionFire.FlexObjects;
using LionFire.IO.Reactive.Hjson;
using LionFire.Reactive.Persistence;

namespace LionFire.IO.Reactive.Filesystem;

public abstract class FsDictionaryProvider : IDictionaryProvider
{
    public DirectorySelector Dir { get; protected set; }

    #region Lifecycle

    public FsDictionaryProvider(DirectorySelector dir)
    {
        Dir = dir;
    }
    public FsDictionaryProvider(IFlex flex)
    {
        Dir = flex.Query<DirectorySelector>()?.Path ?? throw new ArgumentException($"Required: {typeof(DirectorySelector).Name}");
    }

    #endregion

    public abstract IObservableReaderWriter<string, TValue> Get<TValue>() where TValue : notnull;
    public abstract IObservableReader<string, TValue> GetReadOnly<TValue>() where TValue : notnull;
    public abstract IObservableWriter<string, TValue> GetWriteOnly<TValue>() where TValue : notnull;
}

