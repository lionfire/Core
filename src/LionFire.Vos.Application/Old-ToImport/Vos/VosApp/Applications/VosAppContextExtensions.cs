#if TOPORT
namespace LionFire.Vos.VosApp.Old
{
    public static class VosAppContextExtensions
    {
        public static VobHandle<T> ToCurrentContext<T>(this VobHandle<T> h)
            where T : class
        {
            var ctx = VosContext.Current;

            var store = ctx.Store ?? h.EffectiveStore;
            var package = ctx.Package ?? h.EffectivePackage;

            if (h.EffectivePackage == package && h.EffectiveStore == store)
            {
                // l.TraceWarn("OPTIMIZE - don't change Vob since store/pkg match");
                return h;
            }

            var subPath = h.Vob.GetPackageStoreSubPath();
            var vob = ctx.Root[subPath];
            return vob.GetHandle<T>();
        }
    }
}
#endif
