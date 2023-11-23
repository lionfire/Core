
namespace LionFire.Data.Async.Sets;

public class DynamicSetter<TKey, TValue> : SetterSlim<TKey, TValue>
    where TKey : class
    where TValue : class
{
    public Func<TKey, TValue, Task<ISetResult<TValue>>>? Setter { get; set; }

    public override Task<ISetResult<TValue>> SetImpl(TValue value) => Setter(Key, value);
}

