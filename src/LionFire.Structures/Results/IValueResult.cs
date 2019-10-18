namespace LionFire.Results
{
    public interface IValueResult<out TValue>
    {
        TValue Value { get; }
    }
}
