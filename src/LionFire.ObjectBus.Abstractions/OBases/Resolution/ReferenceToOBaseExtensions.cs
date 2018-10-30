using LionFire.DependencyInjection;
using LionFire.MultiTyping;
using LionFire.Referencing;
using LionFire.Structures;

namespace LionFire.ObjectBus
{
    /// <summary>
    /// Extension methods that are typical for resolving ObjectBus objects.
    /// </summary>
    /// <seealso cref="LionFire.ObjectBus.DependencyInjection.OBusResolutionHelpers"/>
    public static class ReferenceToOBaseExtensions
    {
        public static IReferenceToOBaseService OBaseResolverService =>
            ManualSingleton<IReferenceToOBaseService>.Instance ?? InjectionContext.Current.GetService<IReferenceToOBaseService>();

        #region Reference to OBase

        public static (IOBus OBus, IOBase OBase) TryResolve(this IReference reference) => OBaseResolverService.Resolve(reference);
        public static IOBus  TryGetOBus(this IReference reference) => OBaseResolverService.ResolveOBus(reference);
        public static IOBase TryGetOBase(this IReference reference) => OBaseResolverService.Resolve(reference).OBase;

        public static IOBus GetOBus(this IReference reference)
        {
            var result = reference.TryResolve().OBus;

            if (result != null)
            {
                return result;
            }
            
            throw new ObjectBusException("Failed to resolve the IOBus for this reference.  Have you installed the appropriate IOBus for this kind of reference?");
        }

        public static IOBase GetOBase(this IReference reference)
        {
            var result = reference.TryResolve().OBase;

            if (result != null)
            {
                return result;
            }

            throw new ObjectBusException("Failed to resolve the IOBase for this reference.  Have you installed the appropriate IOBus for this kind of reference and connected any relevant IOBases with the correct connection strings?");
        }

        #endregion
    }
}
