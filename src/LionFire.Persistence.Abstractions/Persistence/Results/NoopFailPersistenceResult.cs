
using LionFire.Data.Async.Gets;
using LionFire.Results;

namespace LionFire.Persistence
{
    public struct NoopFailPersistenceResult<TValue> : ITransferResult
    {
        public bool? IsSuccess => false;

        public TValue Value => default;
        public bool HasValue => false;
        public bool IsNoop => true;

        public TransferResultFlags Flags { get => TransferResultFlags.Noop | TransferResultFlags.Fail; set { } }

        public object Error => null;

        public static readonly NoopFailPersistenceResult<TValue> Instance = new NoopFailPersistenceResult<TValue>();
    }
}
