using LionFire.FlexObjects;
using LionFire.IO.Reactive.Filesystem;
using LionFire.Reactive.Persistence;
using System.Collections.Concurrent;

namespace LionFire.IO.Reactive.Hjson;

#if UNUSED
public class HjsonFsDictionaryProvider : FsDictionaryProvider
{

    #region Lifecycle

    public HjsonFsDictionaryProvider(DirectorySelector dir) : base(dir)
    {
    }

    public HjsonFsDictionaryProvider(IFlex flex) : base(flex)
    {
    }

    #endregion

    ConcurrentDictionary<Type, object> cache = new();

    public override IObservableReaderWriter<string, TValue> Get<TValue>()
        => (IObservableReaderWriter<string, TValue>)cache.GetOrAdd(typeof(IObservableReaderWriter<string, TValue>), _ => new ObservableReaderWriterFromComponents<string, TValue>(new HjsonFsDirectoryReaderRx<string, TValue>(Dir), new HjsonFsDirectoryWriterRx<string, TValue>(Dir)));

    public override IObservableReader<string, TValue> GetReadOnly<TValue>()
        => (IObservableReader<string, TValue>)cache.GetOrAdd(typeof(IObservableReader<string, TValue>), _ => new HjsonFsDirectoryReaderRx<string, TValue>(Dir));

    public override IObservableWriter<string, TValue> GetWriteOnly<TValue>()
       => (IObservableWriter<string, TValue>)cache.GetOrAdd(typeof(IObservableWriter<string, TValue>), _ => new HjsonFsDirectoryWriterRx<string, TValue>(Dir));
}
#endif
