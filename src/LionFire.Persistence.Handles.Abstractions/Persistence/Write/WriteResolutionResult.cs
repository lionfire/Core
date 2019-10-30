using LionFire.ObjectBus;
using LionFire.Referencing;

namespace LionFire.Persistence.Resolution
{
    public class WriteResolutionResult<T> : ReferenceResolutionResult
    {
        public WriteResolutionResult() { }
        public WriteResolutionResult(IReference reference) : base(reference) { }

        /// <summary>
        /// See ResolutionResultExtensions.GetHandle.  (REVIEW: Consider making this an interface and making lazy-loading part of the implementation.)
        /// </summary>
        public W<T> Handle { get; set; }
    }
}
