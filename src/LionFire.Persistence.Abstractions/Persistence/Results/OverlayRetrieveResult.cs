
using LionFire.Data.Gets;

namespace LionFire.Persistence;

public class OverlayRetrieveResult<T> : OverlayPersistenceResultBase<ILazyGetResult<T>>, ILazyGetResult<T>, ITieredPersistenceResult
    where T : class
{
    public OverlayRetrieveResult(ILazyGetResult<T> underlyingResult) : base(underlyingResult)
    {
    }

    public T Value => underlyingResult.Value;

    public bool HasValue => Value == default;

}