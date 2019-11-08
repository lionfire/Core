using System.Collections.Generic;

namespace LionFire.Persistence
{

    public class OverlayRetrieveResult<T> : OverlayPersistenceResultBase<IRetrieveResult<T>>, IRetrieveResult<T>, ITieredPersistenceResult
        where T : class
    {
        public OverlayRetrieveResult(IRetrieveResult<T> underlyingResult) : base(underlyingResult)
        {
        }

        public T Value => underlyingResult.Value;

        public bool HasValue => Value == default;


    }
}