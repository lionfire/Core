
using LionFire.Resolves;

namespace LionFire.Persistence
{
    public struct NoopFailPutResult<TValue> : IPutResult
    {
        public bool? IsSuccess => false;
        public bool HasValue => false;
        public TValue Value => default;
        public bool IsNoop => true;

        public PersistenceResultFlags Flags { get => PersistenceResultFlags.Noop | PersistenceResultFlags.Fail; set { } }

        public object Error => null;

        public static readonly NoopFailPutResult<TValue> Instance = new NoopFailPutResult<TValue>();
    }
}
