using System.Collections.Generic;

namespace LionFire.Persistence
{

    public struct TieredRetrieveResult : ITieredPersistenceResult
    {
        public PersistenceResultFlags Flags { get; set; }
        public int RelevantUnderlyingCount { get; set; }
        public IEnumerable<IPersistenceResult> Successes { get; set; }
        public IEnumerable<IPersistenceResult> Failures { get; set; }

        public object Error { get; set; }

        public static readonly TieredRetrieveResult NotFound = new TieredRetrieveResult { Flags = PersistenceResultFlags.NotFound };
    }

    public class TieredRetrieveResult<T> : ITieredRetrieveResult<T>
    {
        public int RelevantUnderlyingCount { get; set; }
        public IEnumerable<IPersistenceResult> Successes { get; set; }
        public IEnumerable<IPersistenceResult> Failures { get; set; }
        public object Error { get; set; }
        public T Object { get; set; }

        public PersistenceResultFlags Flags { get; set; }
    }
}