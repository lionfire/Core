#if UNUSED // Is this useful?
using LionFire.Events;
using System;

namespace LionFire.Data.Async.Gets
{
    public abstract class NotifyingGetterBase<TKey, TValue> : Resolves<TKey, TValue>, ILazilyResolves<TValue>
        //, INotifySenderChanged<TValue>
    {
        public event EventHandler<ValueChanged<TValue>> ValueChanged;

        protected override void OnValueChanged(TValue value, TValue oldValue) => ValueChanged?.Invoke(this, (value, oldValue));
    }

}

#endif