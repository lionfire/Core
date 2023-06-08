
using LionFire.Data.Async.Gets;
using LionFire.Results;

namespace LionFire.Persistence
{
    public struct NoopFailPersistenceResult<TValue> : IPersistenceResult
    {
        public bool? IsSuccess => false;

        public TValue Value => default;
        public bool HasValue => false;
        public bool IsNoop => true;

        public PersistenceResultFlags Flags { get => PersistenceResultFlags.Noop | PersistenceResultFlags.Fail; set { } }

        public object Error => null;

        public static readonly NoopFailPersistenceResult<TValue> Instance = new NoopFailPersistenceResult<TValue>();
    }
}
