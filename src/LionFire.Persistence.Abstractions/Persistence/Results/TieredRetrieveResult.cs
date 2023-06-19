using System.Collections.Generic;
using System.Linq;

namespace LionFire.Persistence
{

    public struct TieredRetrieveResult : ITieredPersistenceResult
    {
        public bool? IsSuccess => Flags.IsSuccessTernary();
        public TransferResultFlags Flags { get; set; }
        public int RelevantUnderlyingCount { get; set; }
        public IEnumerable<ITransferResult> Successes { get; set; }
        public IEnumerable<ITransferResult> Failures { get; set; }
        public bool IsNoop => Flags.HasFlag(TransferResultFlags.Noop);

        public object Error { get; set; }

        public static readonly TieredRetrieveResult NotFound = new TieredRetrieveResult { Flags = TransferResultFlags.NotFound };
    }

    public class TieredRetrieveResult<T> : ITieredRetrieveResult<T>
        where T : class
    {
        public int RelevantUnderlyingCount { get; set; }
        public IEnumerable<ITransferResult> Successes { get; set; }
        public IEnumerable<ITransferResult> Failures { get; set; }
        public object Error { get; set; }

        #region Value

        public T Value
        {
            get => value;
            set => this.value = value;
        }
        private T value;

        #endregion

        public bool IsNoop
        {
            get
            {
                foreach(var child in Successes.Concat(Failures))
                {
                    if (!child.IsNoop) return false;
                }
                return true;
            }
        }

        public bool HasValue => value != default;

        public bool? IsSuccess => Flags.IsSuccessTernary();

        public TransferResultFlags Flags { get; set; }
    }
}