
namespace LionFire.Events
{

    public interface IValueChanged<out TValue>
    {
        TValue NewValue { get; }
        TValue OldValue { get; }
    }
}
