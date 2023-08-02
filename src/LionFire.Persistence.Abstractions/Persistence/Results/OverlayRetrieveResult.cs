
using LionFire.Data.Gets;

namespace LionFire.Persistence;

public class OverlayRetrieveResult<T> : OverlayPersistenceResultBase<IGetResult<T>>, IGetResult<T>, ITieredPersistenceResult
    where T : class
{
    public OverlayRetrieveResult(IGetResult<T> underlyingResult) : base(underlyingResult)
    {
    }

    public T Value => underlyingResult.Value;

    public bool HasValue => Value == default;

}