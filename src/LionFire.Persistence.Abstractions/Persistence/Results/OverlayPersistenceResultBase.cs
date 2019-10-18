using System;
using System.Collections.Generic;

namespace LionFire.Persistence
{
    public class OverlayPersistenceResultBase<TResult> : IPersistenceResult
        where TResult : IPersistenceResult
    {
        protected TResult underlyingResult;
        public OverlayPersistenceResultBase(TResult underlyingResult)
        {
            this.underlyingResult = underlyingResult;
        }

        protected IEnumerable<IPersistenceResult> underlyingEnumerable
        {
            get
            {
                if (_underlyingEnumerable == null)
                {
                    _underlyingEnumerable = new List<IPersistenceResult>{
                        underlyingResult
                    };
                }
                return _underlyingEnumerable;
            }
        }
        IEnumerable<IPersistenceResult> _underlyingEnumerable;

        public int RelevantUnderlyingCount { get; set; }

        public IEnumerable<IPersistenceResult> Successes { get => underlyingResult.IsSuccess() ? underlyingEnumerable : null; set => throw new NotImplementedException(); }
        public IEnumerable<IPersistenceResult> Failures { get => underlyingResult.IsSuccess() ? null : underlyingEnumerable; set => throw new NotImplementedException(); }
        public object Error => underlyingResult.Error;

        public PersistenceResultFlags Flags { get => underlyingResult.Flags; set => throw new NotImplementedException(); }
        public bool? IsSuccess => Flags.IsSuccessTernary();
    }
}