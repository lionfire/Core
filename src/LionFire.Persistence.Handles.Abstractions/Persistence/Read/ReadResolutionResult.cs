
using LionFire.Referencing;

namespace LionFire.Persistence.Resolution
{
    public class ReadResolutionResult<T> : ReferenceResolutionResult
    {
        public ReadResolutionResult() { }
        public ReadResolutionResult(IReference reference) : base(reference) { }
        public ReadResolutionResult(IReadHandleBase<T> readHandle) : base(readHandle.Reference)
        {
            ReadHandle = readHandle;
        }

        /// <summary>
        /// See GetReadHandle (method in ReadResolutionResultExtensions)
        /// </summary>
        public IReadHandleBase<T> ReadHandle { get; set; }
    }

}
