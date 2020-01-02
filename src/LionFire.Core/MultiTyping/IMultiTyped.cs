using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.MultiTyping
{
    public interface IMultiTyped : IReadOnlyMultiTyped
    {
        //void SetType<T>(T obj) where T : class;
        new object this[Type type] { get; set; }
        T AsTypeOrCreateDefault<T>(Type slotType = null) where T : class;
    }


    // TODO REVIEW - IMultiTypedEx may be the commonly used interface, so I don't want it to have Ex on it.  What to do?
    // Call the simple one Minimal?

    //public interface IMultiTypedEx : IMultiTyped
    //{
    //}
}
