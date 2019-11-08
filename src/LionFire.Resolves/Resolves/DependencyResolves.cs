using LionFire.DependencyInjection;
using LionFire.Resolvables;
using MorseCode.ITask;
using System.Collections.Generic;

namespace LionFire.Resolves
{
    public class DependencyResolves<TKey, TValue> : Resolves<TKey, TValue>, ILazilyResolves<TValue>
        where TKey : class
        where TValue : class
    {
        public override ITask<IResolveResult<TValue>> ResolveImpl() 
            => LionFire.ExtensionMethods.Poco.Resolvables.IResolverPocoExtensions.Resolve<TKey, TValue>(Key).AsITask();
    }
}
