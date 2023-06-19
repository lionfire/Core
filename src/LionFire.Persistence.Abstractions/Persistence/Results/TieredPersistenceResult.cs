using System.Collections.Generic;

namespace LionFire.Persistence
{
    public class TieredPersistenceResult : ITieredPersistenceResult
    {
        public TransferResultFlags Flags { get; set; }
        public int RelevantUnderlyingCount { get; set; }
        public IEnumerable<ITransferResult> Successes { get; set; }
        public IEnumerable<ITransferResult> Failures { get; set; }

        public object Error { get; set; }

        public bool? IsSuccess => Flags.IsSuccessTernary();
        public bool IsNoop => Flags.HasFlag(TransferResultFlags.Noop);

        public static readonly TieredPersistenceResult NotFound = new TieredPersistenceResult { Flags = TransferResultFlags.NotFound };
    }
}