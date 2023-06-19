using System;
using System.Collections.Generic;

namespace LionFire.Persistence
{
    public class OverlayPersistenceResultBase<TResult> : ITransferResult
        where TResult : ITransferResult
    {
        protected TResult underlyingResult;
        public OverlayPersistenceResultBase(TResult underlyingResult)
        {
            this.underlyingResult = underlyingResult;
        }

        protected IEnumerable<ITransferResult> underlyingEnumerable
        {
            get
            {
                if (_underlyingEnumerable == null)
                {
                    _underlyingEnumerable = new List<ITransferResult>{
                        underlyingResult
                    };
                }
                return _underlyingEnumerable;
            }
        }
        IEnumerable<ITransferResult> _underlyingEnumerable;

        public int RelevantUnderlyingCount { get; set; }
        public bool IsNoop => underlyingResult.IsNoop;

        public IEnumerable<ITransferResult> Successes { get => underlyingResult.IsSuccess() ? underlyingEnumerable : null; set => throw new NotImplementedException(); }
        public IEnumerable<ITransferResult> Failures { get => underlyingResult.IsSuccess() ? null : underlyingEnumerable; set => throw new NotImplementedException(); }
        public object Error => underlyingResult.Error;

        public TransferResultFlags Flags { get => underlyingResult.Flags; set => throw new NotImplementedException(); }
        public bool? IsSuccess => Flags.IsSuccessTernary();
    }
}