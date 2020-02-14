using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace LionFire
{
    public static class EventHandlingSettings
    {
        public static bool TraceDetachThrowingHandlers = true;

    }


    public static class EventUtils
    {
        //public static bool AutoDetachThrowingHandlers = true;

        /// <summary>
        /// Be sure to cancelableArgs in allArgs so the listener can cancel.
        /// (Uses DynamicInvoke)
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="cancelableArgs"></param>
        /// <param name="allArgs"></param>
        public static void InvokeCancelable(this MulticastDelegate ev, CancelableEventArgs cancelableArgs, params object[] allArgs )
        //where EventType : class
        {
            if (allArgs == null || allArgs.Length == 0)
            {
                allArgs = new object[] { cancelableArgs };
            }

            var evCopy = ev;
            if (evCopy == null) { return; }

            foreach (Delegate del in evCopy.GetInvocationList())
            {
                try
                {
                    del.DynamicInvoke(allArgs);
                }
                catch (Exception ex)
                {
                    {
                        //ev -= del;
                        if (EventHandlingSettings.TraceDetachThrowingHandlers)
                        {
                            l.Error("Event handler threw exception.    Exception: " + ex.ToString());

                            //System.Diagnostics.Trace.WriteLine("Event handler threw exception.    Exception: " + ex.ToString());
                        }
                    }
                }

                if (cancelableArgs.CancelRequested) return;
            }
        }

        private static readonly ILogger l = Log.Get();
		
    }
}
