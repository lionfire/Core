
namespace LionFire.Events
{
    public interface ISenderValueChanged<out TSender, out TValue> : IValueChanged<TValue>
    {
        TSender Sender { get; }
    }
}
