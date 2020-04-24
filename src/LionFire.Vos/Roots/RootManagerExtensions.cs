#nullable enable
using LionFire.Dependencies;
using LionFire.Ontology;
using LionFire.Referencing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Vos
{
    public static class RootManagerExtensions
    {
        private static IVos GetRootManagerOrThrow(IHas<IVos>? hasRootManager = null)
            => hasRootManager?.Object ?? DependencyContext.Current.GetServiceOrSingleton<IVos>(createIfMissing: false)
            ?? throw new System.Exception("Could not find RootManager.  Global DependencyContext may be disabled.  Please specify a IHas<IRootManager> as a parameter.");

        public static IVob? GetVob(this string vosPath, IHas<IVos>? hasRootManager = null) => GetRootManagerOrThrow(hasRootManager).Get(VosConstants.DefaultRootName)?[vosPath.ToVosReference()];


        public static IVob? GetVob(this IVosReference vosReference, IHas<IVos>? hasRootManager = null) 
            => GetRootManagerOrThrow(hasRootManager).Get(vosReference.Persister)?[vosReference.PathChunks];

        public static IVob? ReferencableToVob(this IReferencable<VosReference> vosReferencable, IHas<IVos>? hasRootManager = null) 
            => GetRootManagerOrThrow(hasRootManager).Get(vosReferencable.Reference.Persister)?[vosReferencable.Reference.PathChunks];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vosReference"></param>
        /// <param name="referenceRootVob">If referenceRootVob.RootName matches vosReference.Persister, it will be used as the root vob for finding the path.  Otherwise, referenceRootVob.RootManager.Get(vosReference.Persister) will be used.</param>
        /// <returns></returns>
        public static IVob? GetVob(this IVosReference vosReference, IRootVob referenceRootVob)
        {
            IRootVob? targetRootVob;
            var targetRootName = vosReference.Persister ?? "";

            if (referenceRootVob.RootName == targetRootName) targetRootVob = referenceRootVob;
            else targetRootVob = referenceRootVob.RootManager.Get(vosReference.Persister);

            return targetRootVob?[vosReference.PathChunks];
        }

        public static IVob? GetVob(this IVosReference vosReference, IServiceProvider serviceProvider) => vosReference.GetVob(serviceProvider.GetService<RootManager>());

        public static IVob? QueryVob(this string vosPath, IHas<IVos>? hasRootManager = null) => GetRootManagerOrThrow(hasRootManager).Get(VosConstants.DefaultRootName)?.QueryChild(vosPath.ToVosReference());

        public static IVob? QueryVob(this IVosReference vosReference, IHas<IVos>? hasRootManager = null) => GetRootManagerOrThrow(hasRootManager).Get(vosReference.Persister)?.QueryChild(vosReference.PathChunks);
    }
}
