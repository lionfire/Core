using LionFire.Events;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets
{
    public class DynamicResolves<TKey, TValue> : Resolves<TKey, TValue>, ILazilyResolves<TValue>
        where TKey : class
        where TValue : class
    {
        public Func<TKey, ITask<IResolveResult<TValue>>> Resolver { get; set; }

        protected override ITask<IResolveResult<TValue>> ResolveImpl() => Resolver(Key);
    }
}

