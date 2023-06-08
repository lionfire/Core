using LionFire.Dependencies;
using LionFire.Resolvables;
using MorseCode.ITask;
using System.Collections.Generic;

namespace LionFire.Data.Async.Gets
{
    public class DependencyResolves<TKey, TValue> : Resolves<TKey, TValue>, ILazilyResolves<TValue>
        where TKey : class
        where TValue : class
    {
        protected override ITask<IResolveResult<TValue>> ResolveImpl() 
            => LionFire.ExtensionMethods.Poco.Resolvables.IResolverPocoExtensions.Resolve<TKey, TValue>(Key).AsITask();
    }
}
