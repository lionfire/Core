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
        internal static ConditionalWeakTable<object, IHandle> handles = new ConditionalWeakTable<object, IHandle>();

        

    }
    public static class HandleDictionaryExtensions
    {
        public static IHandle FindHandle(this object obj)
        {
            IHandle h;
            if (HandleDictionary.handles.TryGetValue(obj, out h))
            {
                return h;
            }
            return null;
        }

        public static bool TryRegisterHandle(this IHandle h)
        {
            object o = h.Object;

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
        public static void RegisterHandle(this IHandle h)
        {
            object o = h.Object;
            HandleDictionary.handles.Add(o, h);
        }
    }

}
