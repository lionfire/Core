
namespace LionFire.Events
{
    public struct ValueChanged<T>
    {
        public ValueChanged(T newValue, T oldValue = default)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }

        public static implicit operator ValueChanged<T>((T newValue, T oldValue) tuple) => new ValueChanged<T>(tuple.newValue, tuple.oldValue);
        public static implicit operator ValueChanged<T>(T newValue) => new ValueChanged<T>(newValue);

        public T NewValue { get; set; }
        public T OldValue { get; set; }
    }
}
