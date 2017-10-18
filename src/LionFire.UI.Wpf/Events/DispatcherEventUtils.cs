using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;

namespace LionFire
{
    public static class DispatcherEventUtils
    {
        public static void DispatchDynamicInvoke(this MulticastDelegate ev, params object[] args)
        //where EventType : class
        {

            var evCopy = ev;
            if (evCopy == null) { return; }

            foreach (Delegate del in evCopy.GetInvocationList())
            {
                try
                {
                    if (del.Target as DispatcherObject != null)
                    {
                        DispatcherObject dispatcherObject = (DispatcherObject)del.Target;

                        // REVIEW - Changed to BeginInvoke.  No point in waiting for the invocation to finish! (Unless perhaps we wanted to do error handling.)
                        if (dispatcherObject.Dispatcher != null && !dispatcherObject.Dispatcher.CheckAccess())
                        {
                            dispatcherObject.Dispatcher.BeginInvoke(del, args);
                            continue;
                        }
                    }
                    del.DynamicInvoke(args);
                }
                catch (Exception ex)
                {
                    //if (AutoDetachThrowingHandlers)
                    {
                        //ev -= del;
                        if (EventHandlingSettings.TraceDetachThrowingHandlers)
                        {
                            l.Error("Event handler threw exception.    Exception: " + ex.ToString());
                        }
                    }
                }
            }
        }
        private static ILogger l = Log.Get();
    }
}
