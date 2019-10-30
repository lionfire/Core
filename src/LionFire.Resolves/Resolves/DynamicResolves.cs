using LionFire.Events;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public class DynamicResolves<TKey, TValue> : Resolves<TKey, TValue>, ILazilyResolves<TValue>
    {
        public Func<TKey, ITask<IResolveResult<TValue>>> Resolver { get; set; }

        public override ITask<IResolveResult<TValue>> ResolveImpl() => Resolver(Key);
    }
}

