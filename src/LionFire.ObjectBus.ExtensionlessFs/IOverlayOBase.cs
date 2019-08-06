using LionFire.Persistence.Resolution;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.ObjectBus.ExtensionlessFs
{
    public interface IOverlayOBase : IOBase
    {
         IOBase UnderlyingOBase { get; }
    }

    public interface IOverlayOBase<TOverlayReference, TUnderlyingReference> : IOverlayOBase
    {
        IReferenceToReferenceResolver ReferenceResolutionStrategy { get; }
        Task<IEnumerable<ReadResolutionResult<TObject>>> GetUnderlyingReferences<TObject>(TOverlayReference overlayReference);
    }
}
