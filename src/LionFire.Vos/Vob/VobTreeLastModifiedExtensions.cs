using System;
using System.Threading.Tasks;

namespace LionFire.Vos
{
    public static class VobTreeLastModifiedExtensions
    {
        public static  Task UpdateTreeLastModified(this IVob vob)
        {
            throw new NotImplementedException();
#if DISABLED
            var vh = vob.GetVHTreeLastModified();
            (await vh.GetOrInstantiate().ConfigureAwait(false)).DateTime = DateTime.Now;
            await vh.Commit().ConfigureAwait(false);
#endif
        }

#if DISABLED
        public static VobHandle<TimeStamp> GetVHTreeLastModified(this Vob vob) => vob[VosPaths.MetaDataSubPath]["TreeLastModified"].GetHandle<TimeStamp>();
#endif
        public static DateTime? GetTreeLastModified(this IVob vob)
        {
            throw new NotImplementedException();
#if DISABLED
            vob.GetVHTreeLastModified().DiscardValue();
            var obj = vob.GetVHTreeLastModified().Value;
            //if (obj != null) { l.Trace("[lastmodified] " + obj.DateTime.ToStringSafe() + " " + this); }
            return obj == null ? null : (DateTime?)obj.DateTime;
#endif
        }
    }
}