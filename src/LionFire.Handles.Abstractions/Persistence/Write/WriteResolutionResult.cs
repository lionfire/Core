using LionFire.ObjectBus;

namespace LionFire.Referencing.Persistence
{
    public class WriteResolutionResult<T> : ReferenceResolutionResult
    {
        public WriteResolutionResult() { }
        public WriteResolutionResult(IReference reference) : base(reference) { }

        /// <summary>
        /// See ResolutionResultExtensions.GetHandle.  (REVIEW: Consider making this an interface and making lazy-loading part of the implementation.)
        /// </summary>
        public H<T> Handle { get; set; }
    }
}
