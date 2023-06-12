using LionFire.Dependencies;
using MorseCode.ITask;
using System.Collections.Generic;

namespace LionFire.Data.Async.Gets;

public class DependencyResolves<TKey, TValue> : Gets<TKey, TValue>, ILazilyGets<TValue>
    where TKey : class
    where TValue : class
{
    protected override ITask<IGetResult<TValue>> GetImpl() 
        => LionFire.ExtensionMethods.Poco.Resolvables.IResolverPocoExtensions.Resolve<TKey, TValue>(Key).AsITask();
}
