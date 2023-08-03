using LionFire.ExtensionMethods.Poco.Data.Async;

namespace LionFire.Data.Async.Sets;

public class AmbientSetter<TKey, TValue> : SetterSlim<TKey, TValue>
     where TKey : class
    where TValue : class
{
    public override Task<ITransferResult> SetImpl(TValue value) => this.Key.Set(value);
}

