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
        internal static ConditionalWeakTable<object, W<object>> handles = new ConditionalWeakTable<object, W<object>>();
    }
    
    public static class HandleDictionaryExtensions
    {
        public static W<object> FindHandle(this object obj) 
            => HandleDictionary.handles.TryGetValue(obj, out W<object> h) ? h : null;

        public static bool TryRegisterHandle(this W<object> h)
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
        public static void RegisterHandle(this W<object> h)
        {
            object o = h.Value;
            HandleDictionary.handles.Add(o, h);
        }
    }

}
