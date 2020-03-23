namespace LionFire.Persistence
{
    public struct NoopRetrieveResult<T> : IRetrieveResult<T>
    {
        public static NoopRetrieveResult<T> Instance => new NoopRetrieveResult<T>();

        public bool? IsSuccess => false;

        public T Value => default;

        public bool HasValue => false;

        public PersistenceResultFlags Flags
        {
            get => PersistenceResultFlags.Noop | PersistenceResultFlags.Fail;
            set { }
        }
        public object Error => null;

        public bool IsNoop => true;
    }

}