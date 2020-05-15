#nullable enable
using LionFire.Ontology;
using System;

namespace LionFire.Vos
{
    /// <summary>
    /// Provides access to the root Vob (or named root Vobs).
    /// </summary>
    public interface IVos : IHas<IVos>, IHas<IServiceProvider>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootName"></param>
        /// <returns>The requested IRootVob, or null if IRootVob has not been created.  (use IServiceCollection.AddVosRoot extension method to add additional IRootVobs during application initialization.)</returns>
        IRootVob? Get(string? rootName = null);

        VosOptions Options { get; }
    }

    public static class IRootManagerExtensions
    {
        public static IVob ThrowVobNotAvailableException(string vobPath)
            => throw new ArgumentException($"Vob not available.  Did you register the root name in VosOptions?  Vob: {vobPath}");

        // OPTIMIZE - make this a method in IRootManager to avoid traversing to the unnamed IRootVob (which may not exist) and then back to the root manager using the /../rootName/... syntax.
        public static IVob GetVob(this IVos rootManager, string vobPath) => rootManager.Get(null)?[vobPath] ?? ThrowVobNotAvailableException(vobPath);
        public static IVob GetVob(this IVos rootManager, IVosReference vobReference) => rootManager.Get(null)?[vobReference] ?? ThrowVobNotAvailableException(vobReference.Path);
        public static IVob? TryGetVob(this IVos rootManager, string vobPath) => rootManager.Get(null)?[vobPath];
        public static IVob? TryGetVob(this IVos rootManager, IVosReference vobReference) => rootManager.Get(null)?[vobReference];
    }
}
