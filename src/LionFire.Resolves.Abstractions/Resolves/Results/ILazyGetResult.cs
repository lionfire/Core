using LionFire.Results;

namespace LionFire.Data.Gets;

public interface ILazyGetResult<out TValue> : IGetResult<TValue> // TODO: Replace ILazyGetResult with IGetResult
{
}
