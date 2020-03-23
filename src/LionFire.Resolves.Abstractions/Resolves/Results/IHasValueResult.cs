using LionFire.Results;

namespace LionFire.Resolves
{
    public interface IHasDefaultableValueResult<TValue> : IHasValueResult, IValueResult<TValue>
    {
    }

    public interface IHasValueResult
    {
        bool HasValue { get; }
    }
}
