
namespace LionFire.Events
{
    public interface ISenderValueChanged<out TValue, out TSender>: IValueChanged<TValue>
    {
        TSender Sender { get; }
    }
}
