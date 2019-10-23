using LionFire.Events;
using System;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public class Resolves<TKey, TValue> : ResolvesBase<TKey, TValue>, ILazilyResolves<TValue>
    {
        public Func<TKey, Task<IResolveResult<TValue>>> Resolver { get; set; }

        public override Task<IResolveResult<TValue>> ResolveImpl() => Resolver(Key);
    }
}

