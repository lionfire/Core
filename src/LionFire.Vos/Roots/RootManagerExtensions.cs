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
            => hasRootManager?.Object ?? DependencyContext.Current.GetService<IVos>()
            ?? throw new System.Exception("Could not find RootManager.  Global DependencyContext may be disabled.  Please specify a IHas<IRootManager> as a parameter.");

        public static IVob? GetVob(this string vosPath, IHas<IVos>? hasRootManager = null) => GetRootManagerOrThrow(hasRootManager).Get(VosConstants.DefaultRootName)?[vosPath.ToVobReference()];


        public static IVob? GetVob(this IVobReference vobReference, IHas<IVos>? hasRootManager = null) 
            => GetRootManagerOrThrow(hasRootManager).Get(vobReference.Persister)?[vobReference.PathChunks];

        public static IVob? ReferencableToVob(this IReferencable<VobReference> vosReferencable, IHas<IVos>? hasRootManager = null) 
            => GetRootManagerOrThrow(hasRootManager).Get(vosReferencable.Reference.Persister)?[vosReferencable.Reference.PathChunks];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vobReference"></param>
        /// <param name="referenceRootVob">If referenceRootVob.RootName matches vobReference.Persister, it will be used as the root vob for finding the path.  Otherwise, referenceRootVob.RootManager.Get(vobReference.Persister) will be used.</param>
        /// <returns></returns>
        public static IVob? GetVob(this IVobReference vobReference, IRootVob referenceRootVob)
        {
            IRootVob? targetRootVob;
            var targetRootName = vobReference.Persister ?? "";

            if (referenceRootVob.RootName == targetRootName) targetRootVob = referenceRootVob;
            else targetRootVob = referenceRootVob.RootManager.Get(vobReference.Persister);

            return targetRootVob?[vobReference.PathChunks];
        }

        public static IVob? GetVob(this IVobReference vobReference, IServiceProvider serviceProvider) => vobReference.GetVob(serviceProvider.GetService<RootManager>());

        public static IVob? QueryVob(this string vosPath, IHas<IVos>? hasRootManager = null) => GetRootManagerOrThrow(hasRootManager).Get(VosConstants.DefaultRootName)?.QueryChild(vosPath.ToVobReference());

        public static IVob? QueryVob(this IVobReference vobReference, IHas<IVos>? hasRootManager = null) => GetRootManagerOrThrow(hasRootManager).Get(vobReference.Persister)?.QueryChild(vobReference.PathChunks);
    }
}
