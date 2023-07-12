using LionFire.ExtensionMethods.Poco.Data.Async;

namespace LionFire.Data.Sets;

public class AmbientSets<TKey, TValue> : AsyncSetsSlim<TKey, TValue>
     where TKey : class
    where TValue : class
{
    public override Task<ITransferResult> SetImpl(TValue value) => this.Key.Set(value);
}

