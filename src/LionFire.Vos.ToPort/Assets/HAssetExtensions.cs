using LionFire.Persistence;
using System;

namespace LionFire.Assets
{
    public static class HAssetExtensions
    {
#if !AOT
        public static T TryGetObject<T>(this HAsset<T> ha)
            where T : class
        {
            if (ha == null) return null;
            return ha.Value;
        }
#endif

        [AotReplacement] // First param is different, not sure it works :-/
        public static object TryGetObject(this IReadHandleBase<object> ha, Type type) => ha == null ? null : ha.Value;
    }
}
