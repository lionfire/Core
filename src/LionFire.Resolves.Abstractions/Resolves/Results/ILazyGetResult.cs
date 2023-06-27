using LionFire.Results;

namespace LionFire.Data.Async.Gets;

public interface ILazyGetResult<out TValue> : IGetResult<TValue>
{
}
