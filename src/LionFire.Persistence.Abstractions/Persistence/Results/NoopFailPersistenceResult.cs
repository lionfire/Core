
using LionFire.Data;
using LionFire.Data.Async.Gets;
using LionFire.Results;

namespace LionFire.Persistence
{
    public struct NoopFailPersistenceResult<TValue> : ITransferResult
    {
        public static readonly NoopFailPersistenceResult<TValue> Instance = new NoopFailPersistenceResult<TValue>();

        public bool? IsSuccess => false;

        public TValue Value => default;
        public bool HasValue => false;

        public TransferResultFlags Flags { get => TransferResultFlags.Noop | TransferResultFlags.Fail; }

    }
}
