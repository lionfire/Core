
namespace LionFire.Events
{

    public interface IValueChanged<out TValue>
    {
        TValue New { get; }
        TValue Old { get; }
    }
}
