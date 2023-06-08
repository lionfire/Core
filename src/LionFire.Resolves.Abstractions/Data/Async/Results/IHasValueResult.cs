using LionFire.Results;

namespace LionFire.Data.Async.Gets;

#if UNUSED
public interface IHasDefaultableValueResult<TValue> : IHasValueResult, IValueResult<TValue>
{
}
#endif


public interface IHasValueResult
{
    bool HasValue { get; }
}
