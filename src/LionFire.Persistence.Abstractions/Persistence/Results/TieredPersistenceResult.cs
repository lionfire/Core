using System.Collections.Generic;

namespace LionFire.Persistence
{
    public class TieredPersistenceResult : ITieredPersistenceResult
    {
        public PersistenceResultFlags Flags { get; set; }
        public int RelevantUnderlyingCount { get; set; }
        public IEnumerable<IPersistenceResult> Successes { get; set; }
        public IEnumerable<IPersistenceResult> Failures { get; set; }

        public object Error { get; set; }

        public bool? IsSuccess => Flags.IsSuccessTernary();

        public static readonly TieredPersistenceResult NotFound = new TieredPersistenceResult { Flags = PersistenceResultFlags.NotFound };
    }
}