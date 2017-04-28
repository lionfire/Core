using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Assets;

namespace LionFire.ObjectBus
{
    public static class VosExtensions
    {
        public static VobHandle<T> ToHandle<T>(this Vob vob)
            where T : class
        {
            if (vob == null) return null;
            return vob.GetHandle<T>();
        }

        public static IVobHandle ToHandle(this Vob vob, Type type = null)
        {
            if (vob == null) return null;
            return vob.GetHandle(type);
        }

        //private static ILogger l = Log.Get();
		
    }
}
