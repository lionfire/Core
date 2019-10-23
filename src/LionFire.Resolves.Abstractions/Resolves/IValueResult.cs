namespace LionFire.Resolves
{
    public interface IValueResult<out TValue>
    {
        TValue Value { get; }
    }
}
