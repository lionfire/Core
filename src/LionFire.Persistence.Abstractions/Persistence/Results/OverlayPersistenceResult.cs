namespace LionFire.Persistence
{
    public class OverlayPersistenceResult : OverlayPersistenceResultBase<ITransferResult>
    {
        public OverlayPersistenceResult(ITransferResult underlyingResult) : base(underlyingResult)
        {
        }
    }
}