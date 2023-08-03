using LionFire.ExtensionMethods.Poco.Getters;

namespace LionFire.Data.Async.Gets;

public class AmbientGetter<TKey, TValue> : Getter<TKey, TValue>
     where TKey : class
    where TValue : class
{
    public AmbientGetter(TKey key, GetterOptions? options = null) : base(key, options)
    {
    }

    protected override ITask<IGetResult<TValue>> GetImpl(CancellationToken cancellationToken = default) 
        => this.Key.AmbientGet<TKey, TValue>().AsITask();
}

