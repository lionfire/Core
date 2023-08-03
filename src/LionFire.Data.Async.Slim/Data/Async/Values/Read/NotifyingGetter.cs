#if UNUSED // Is this useful?
using LionFire.Events;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets
{
    public class NotifyingGetter<TKey, TValue> : NotifyingGetterBase<TKey, TValue>, ILazyGetter<TValue>, INotifySenderChanged<TValue>
    {
        public Func<TKey, ITask<IResolveResult<TValue>>> Resolver { get; set; }

        public override ITask<IResolveResult<TValue>> GetImpl(CancellationToken cancellationToken = default) => Resolver(this.Key);
    }

}

#endif