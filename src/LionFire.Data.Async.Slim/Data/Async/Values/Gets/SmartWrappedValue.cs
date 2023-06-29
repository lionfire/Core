
namespace LionFire.Data.Gets;

//public class SmartWrappedValue<TValue>
//{
//    public TValue ReadCacheValue
//    {
//        get => readCacheValue;
//        set
//        {
//            if (EqualityComparer<TValue>.Default.Equals(readCacheValue, value)) return;
//            var oldValue = readCacheValue;

//            ValueChangedPropagation.Detach(readCacheValue);
//            readCacheValue = value;
//            WrappedValueForFromTo?.Invoke(this, oldValue, readCacheValue);
//            ValueChangedPropagation.Attach(readCacheValue, o => WrappedValueChanged?.Invoke(this));

//            WrappedValueChanged?.Invoke(this); // Assume that there was a change

//            OnValueChanged(value, oldValue);
//        }
//    }
//    /// <summary>
//    /// Raw field for readCacheValue.  Should typically call OnValueChanged(TValue newValue, TValue oldValue) after this field changes.
//    /// </summary>
//    protected TValue readCacheValue;

//    public event Action<INotifyWrappedValueReplaced, object, object> WrappedValueForFromTo;
//    public event Action<INotifyWrappedValueChanged> WrappedValueChanged;
//}

