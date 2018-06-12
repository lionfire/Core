using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Threading
{
    public static class DispatcherUtils 
    {
        // FUTURE: Integrate with InjectionContext?

        public static List<IDispatcherProvider> DispatcherProviders
        {
            get => dispatcherProviders;
        }
        static List<IDispatcherProvider> dispatcherProviders = new List<IDispatcherProvider>();

        public static IDispatcher TryGetDispatcher(this object obj)
        {
            foreach (var provider in DispatcherProviders)
            {
                var disp = provider.DispatcherFor(obj);
                if (disp != null) return disp;
            }
            return null;
        }
    }

}
