using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public class HandleDictionary
    {
        internal static ConditionalWeakTable<object, IReadWriteHandleBase<object>> handles = new ConditionalWeakTable<object, IReadWriteHandleBase<object>>();
    }
    
    public static class HandleDictionaryExtensions
    {
        public static IReadWriteHandleBase<object> FindHandle(this object obj) 
            => HandleDictionary.handles.TryGetValue(obj, out IReadWriteHandleBase<object> h) ? h : null;

        public static bool TryRegisterHandle(this IReadWriteHandleBase<object> h)
        {
            object o = h.Value;

            try
            {
                HandleDictionary.handles.Add(o, h);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
        public static void RegisterHandle(this IReadWriteHandleBase<object> h)
        {
            object o = h.Value;
            HandleDictionary.handles.Add(o, h);
        }
    }

}
