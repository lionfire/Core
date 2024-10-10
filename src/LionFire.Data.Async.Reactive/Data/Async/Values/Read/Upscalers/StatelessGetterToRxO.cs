
namespace LionFire.Data.Async.Gets;

public class StatelessGetterToRxO<TValue>
    : ReactiveObject
    , IGetterRxO<TValue>
    , IMightFireChangeEvents
{
    IStatelessGetter<TValue> source;

    public bool FiresChangeEvents => false;

    public StatelessGetterToRxO(IStatelessGetter<TValue> getter)
    {
        source = getter;
    }

    public TValue? ReadCacheValue => throw new NotImplementedException();

    public TValue? Value => throw new NotImplementedException();

    public bool HasValue => throw new NotImplementedException();

    public IObservable<IGetResult<TValue>> GetResults => throw new NotImplementedException();

    public IObservable<ITask<IGetResult<TValue>>> GetOperations => throw new NotImplementedException();

    public void Discard()
    {
        throw new NotImplementedException();
    }

    public void DiscardValue()
    {
        throw new NotImplementedException();
    }

    public ITask<IGetResult<TValue>> Get(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ITask<IGetResult<TValue>> GetIfNeeded()
    {
        throw new NotImplementedException();
    }

    public IGetResult<TValue> QueryGetResult()
    {
        throw new NotImplementedException();
    }
}
