using LionFire.ExtensionMethods.Poco.Resolvables;

namespace LionFire.Data.Gets;

public class AmbientGets<TKey, TValue> : AsyncGets<TKey, TValue>
     where TKey : class
    where TValue : class
{
    public AmbientGets(TKey key, AsyncValueOptions? options = null) : base(key, options)
    {
    }

    protected override ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default) 
        => this.Key.AmbientGet<TKey, TValue>().AsITask();
}

