
namespace LionFire.Resolves;

//public class SmartWrappedValue<TValue>
//{
//    public TValue ProtectedValue
//    {
//        get => protectedValue;
//        set
//        {
//            if (EqualityComparer<TValue>.Default.Equals(protectedValue, value)) return;
//            var oldValue = protectedValue;

//            ValueChangedPropagation.Detach(protectedValue);
//            protectedValue = value;
//            WrappedValueForFromTo?.Invoke(this, oldValue, protectedValue);
//            ValueChangedPropagation.Attach(protectedValue, o => WrappedValueChanged?.Invoke(this));

//            WrappedValueChanged?.Invoke(this); // Assume that there was a change

//            OnValueChanged(value, oldValue);
//        }
//    }
//    /// <summary>
//    /// Raw field for protectedValue.  Should typically call OnValueChanged(TValue newValue, TValue oldValue) after this field changes.
//    /// </summary>
//    protected TValue protectedValue;

//    public event Action<INotifyWrappedValueReplaced, object, object> WrappedValueForFromTo;
//    public event Action<INotifyWrappedValueChanged> WrappedValueChanged;
//}

