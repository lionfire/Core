using LionFire.Resolves;

namespace LionFire.Persistence
{
    public struct NoopFailDeleteResult<TValue> : IDeleteResult
    {
        public bool? IsSuccess => false;
        public bool HasValue => false;
        public TValue Value => default;
        public bool IsNoop => true;

        public PersistenceResultFlags Flags { get => PersistenceResultFlags.Fail | PersistenceResultFlags.Noop; set { } }

        public object Error => null;

        public static readonly NoopFailDeleteResult<TValue> Instance = new NoopFailDeleteResult<TValue>();

    }

}
