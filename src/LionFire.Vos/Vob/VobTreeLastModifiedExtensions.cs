﻿using System;
using System.Threading.Tasks;

namespace LionFire.Vos
{
    public static class VobTreeLastModifiedExtensions
    {
        public static async Task UpdateTreeLastModified(this Vob vob)
        {
            var vh = vob.GetVHTreeLastModified();
            (await vh.GetOrInstantiate().ConfigureAwait(false)).DateTime = DateTime.Now;
            await vh.Commit().ConfigureAwait(false);
        }

        public static VobHandle<TimeStamp> GetVHTreeLastModified(this Vob vob) => vob[VosPaths.MetaDataSubPath]["TreeLastModified"].GetHandle<TimeStamp>();

        public static DateTime? GetTreeLastModified(this Vob vob)
        {
            vob.GetVHTreeLastModified().DiscardValue();
            var obj = vob.GetVHTreeLastModified().Value;
            //if (obj != null) { l.Trace("[lastmodified] " + obj.DateTime.ToStringSafe() + " " + this); }
            return obj == null ? null : (DateTime?)obj.DateTime;
        }
    }
}