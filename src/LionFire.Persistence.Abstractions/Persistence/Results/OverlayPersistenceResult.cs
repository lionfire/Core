namespace LionFire.Persistence
{
    public class OverlayPersistenceResult : OverlayPersistenceResultBase<IPersistenceResult>
    {
        public OverlayPersistenceResult(IPersistenceResult underlyingResult) : base(underlyingResult)
        {
        }
    }
}