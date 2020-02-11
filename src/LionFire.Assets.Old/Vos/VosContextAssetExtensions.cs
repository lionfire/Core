using System;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos
{
    public static class VosContextAssetExtensions
    {
        #region Assets

        public static Vob ToAssetVob(this string assetPath) => VosContext.Current.Root[assetPath];

        public static IVobHandle ToAssetVobHandle(this string assetPath, Type type)
        {
            if (VosContext.Current == null)
            {
                throw new UnreachableCodeException("VosContext.Current == null");
            }
            if (VosContext.Current.Root == null)
            {
                throw new UnreachableCodeException("VosContext.Current.Root == null");
            }
            // REvIEW - should this be in a asset subpath????
            return VosContext.Current.Root[assetPath].GetHandle(type);
        }
        public static VobHandle<T> ToAssetVobHandle<T>(this string assetPath)
            where T : class
        {
            if (VosContext.Current == null)
            {
                throw new UnreachableCodeException("VosContext.Current == null");
            }
            if (VosContext.Current.Root == null)
            {
                throw new UnreachableCodeException("VosContext.Current.Root == null");
            }
            return VosContext.Current.Root[assetPath].GetHandle<T>();
        }

        #endregion

        
        private static ILogger l = Log.Get();

    }
}

