namespace LionFire.Persistence
{
    public struct NoopPutPersistenceResult : IPersistenceResult
    {
        public PersistenceResultFlags Flags { get => PersistenceResultFlags.Noop | PersistenceResultFlags.Success; set => throw new System.NotImplementedException(); }

        public object Error => "Noop implementation";

        public bool? IsSuccess => false;
        public bool IsNoop => true;

        public static NoopPutPersistenceResult Instance { get; } = new NoopPutPersistenceResult();
    }
}