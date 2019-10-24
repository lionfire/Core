
namespace LionFire.Events
{
    public interface IValueChanged<out T>
    {
        T NewValue { get; }
        T OldValue { get; }
    }
}
