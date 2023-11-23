using LionFire.Results;

namespace LionFire.Results;

#if UNUSED
public interface IHasDefaultableValueResult<TValue> : IHasValueResult, IValueResult<TValue>
{
}
#endif

/// <summary>
/// Indicates whether an object has a value or not
/// </summary>
public interface IHasValueResult
{
    bool HasValue { get; }
}
