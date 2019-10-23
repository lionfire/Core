using LionFire.Events;
using System;

namespace LionFire.Resolves
{
    public abstract class NotifyingResolvesBase<TKey, TValue> : ResolvesBase<TKey, TValue>, ILazilyResolves<TValue>, INotifiesSenderValueChanged<TValue>
    {
        public event EventHandler<ValueChanged<TValue>> ValueChanged;

        protected override void OnValueChanged(TValue value, TValue oldValue) => ValueChanged?.Invoke(this, (value, oldValue));
    }

}

