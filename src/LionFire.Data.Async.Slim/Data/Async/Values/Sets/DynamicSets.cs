using LionFire.Dependencies;
using LionFire.Resolvers;
using System;
using System.Threading.Tasks;
using LionFire.ExtensionMethods;
using LionFire.Results;
using LionFire.Persistence;

namespace LionFire.Data.Async.Sets;

public class DynamicSets<TKey, TValue> : AsyncSetsSlim<TKey, TValue>
    where TKey : class
    where TValue : class
{
    public Func<TKey, TValue, Task<ITransferResult>>? Committer { get; set; }

    public override Task<ITransferResult> SetImpl(TValue value) => Committer(Key, value);
}

