using LionFire.Events;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public class NotifyingResolves<TKey, TValue> : NotifyingResolvesBase<TKey, TValue>, ILazilyResolves<TValue>, INotifySenderChanged<TValue>
    {
        public Func<TKey, ITask<IResolveResult<TValue>>> Resolver { get; set; }

        public override ITask<IResolveResult<TValue>> ResolveImpl() => Resolver(this.Key);
    }

}

