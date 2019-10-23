using LionFire.Events;
using System;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public class NotifyingResolves<TKey, TValue> : NotifyingResolvesBase<TKey, TValue>, ILazilyResolves<TValue>, INotifiesSenderValueChanged<TValue>
    {
        public Func<TKey, Task<IResolveResult<TValue>>> Resolver { get; set; }

        public override Task<IResolveResult<TValue>> ResolveImpl() => Resolver(this.Key);
    }

}

