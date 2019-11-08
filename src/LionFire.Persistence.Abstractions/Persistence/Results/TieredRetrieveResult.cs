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
        where T : class
    {
        public int RelevantUnderlyingCount { get; set; }
        public IEnumerable<IPersistenceResult> Successes { get; set; }
        public IEnumerable<IPersistenceResult> Failures { get; set; }
        public object Error { get; set; }

        #region Value

        public T Value
        {
            get => value;
            set => this.value = value;
        }
        private T value;

        #endregion


        public bool HasValue => value != default;

        public bool? IsSuccess => Flags.IsSuccessTernary();

        public PersistenceResultFlags Flags { get; set; }
    }
}