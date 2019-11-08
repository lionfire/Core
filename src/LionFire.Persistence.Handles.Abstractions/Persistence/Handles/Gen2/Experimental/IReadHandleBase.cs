#if Experimental
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Handles.Experimental
{
    public interface IReadHandleBase
    {
    }
    public static class IReadHandleBaseExtensions
    {
        public static IReadHandle<object> ToObjectHandle(this IReadHandleBase rhb)
        {
            var rh = (IReadHandle<object>)rhb;
            return rh;
        }
    }
}
#endif
