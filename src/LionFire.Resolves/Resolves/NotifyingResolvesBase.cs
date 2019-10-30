using LionFire.Events;
using System;

namespace LionFire.Resolves
{
    public abstract class NotifyingResolvesBase<TKey, TValue> : Resolves<TKey, TValue>, ILazilyResolves<TValue>, INotifySenderChanged<TValue>
    {
        public event EventHandler<ValueChanged<TValue>> ValueChanged;

        protected override void OnValueChanged(TValue value, TValue oldValue) => ValueChanged?.Invoke(this, (value, oldValue));
    }

}

