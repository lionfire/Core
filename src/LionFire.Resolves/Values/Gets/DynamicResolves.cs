using LionFire.Events;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets
{
    public class DynamicResolves<TKey, TValue> : Gets<TKey, TValue>, ILazilyGets<TValue>
        where TKey : class
        where TValue : class
    {
        public Func<TKey, ITask<IGetResult<TValue>>> Resolver { get; set; }

        protected override ITask<IGetResult<TValue>> GetImpl() => Resolver(Key);
    }
}

