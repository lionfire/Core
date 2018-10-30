using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Vos
{
    public interface IHasVobHandle
    {
        IVobHandle VobHandle { get; }
    }

#if !AOT
    public interface IHasVobHandle<T> : IHasVobHandle
        where T : class // , new()
    {
        new VobHandle<T> VobHandle { get; }
    }
#endif

    public interface ICanSetVobHandle
    {
        IVobHandle VobHandle { set; }
    }

    public static class IHasVobHandlePersistenceExtensions
    {
        //public static void Save(this IHasVobHandle hasHandle)
        //{
        //    if (hasHandle == null) throw new ArgumentNullException("hasHandle");
        //    if (hasHandle.VobHandle == null) throw new ArgumentNullException("hasHandle.VobHandle ");
        //    if (hasHandle.VobHandle.Vob == null) throw new ArgumentNullException("hasHandle.VobHandle.Vob");

        //    l.Trace("REVIEW - IHasVobHandlePersistenceExtensions.Save");
            
        //    hasHandle.VobHandle.Vob.Save(); - No more type-less save
        //}
        private static ILogger l = Log.Get();
		

    }

}
