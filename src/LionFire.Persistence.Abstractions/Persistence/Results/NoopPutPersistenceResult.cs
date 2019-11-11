namespace LionFire.Persistence
{
    public struct NoopPutPersistenceResult : IPutPersistenceResult
    {
        public PersistenceResultFlags Flags { get => PersistenceResultFlags.Noop | PersistenceResultFlags.Success; set => throw new System.NotImplementedException(); }

        public object Error => null;

        public bool? IsSuccess => true;

        public static NoopPutPersistenceResult Instance { get; } = new NoopPutPersistenceResult();
    }
}