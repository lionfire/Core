#define SanityChecks
// Retrieved from http://karlhulme.wordpress.com/2007/03/04/synchronizedobservablecollection-and-bindablecollection/ on Feb 7, 2010
// License: If anyone can make use of the above classes then you’re very welcome to use them, although you do so at your own risk! 
using System.Collections.Specialized;
using System;
using LionFire.Threading;
using System.Diagnostics;
#if UNITY
using LionFire.Backports;
#endif

namespace LionFire.Collections
{
    public static class MultiBindableEvents
    {
        public static bool AutoDetachThrowingHandlers = true;
        public static bool TraceDetachThrowingHandlers = true;
        public static bool RaiseResetOnThrow = true;

        public static void RaiseCollectionChangedEventNonGeneric(this NotifyCollectionChangedEventHandler ev, object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ev == null)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldStartingIndex < 0)
            {
                Trace.WriteLine("[warn] Replacing NotifyCollectionChangedAction.Remove event with Reset because there is no index");
                MultiBindableEvents.RaiseCollectionChangedEventNonGeneric(ev, sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                return;
            }

            foreach (NotifyCollectionChangedEventHandler del in ev.GetInvocationList())
            {
                try
                {

                    IDispatcher dispatcher = del.Target.TryGetDispatcher();

                    if (dispatcher != null && dispatcher.IsInvokeRequired)
                    {
                        dispatcher.Invoke(() => del(sender, e));
                        //dispatcher.Invoke(del, new object[] { sender, e }); // OLD
                        continue;
                    }

#if OLD // WindowsBase
                    DispatcherObject dispatcherObject = del.Target as DispatcherObject;
					if(dispatcherObject != null) {
						if(dispatcherObject.Dispatcher != null && !dispatcherObject.Dispatcher.CheckAccess()) {
							dispatcherObject.Dispatcher.Invoke(del, new object[] { sender, e });
//                            dispatcherObject.Dispatcher.Invoke(del, sender, e);
							continue;
						}
					}
#endif
                    del.Invoke(sender, e);
                }
                catch (Exception ex)
                {
                    if ((ex as ArgumentOutOfRangeException != null) && RaiseResetOnThrow)
                    {
                        try
                        {
                            del.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                            continue;
                        }
                        catch (Exception ex2)
                        {
                            Trace.WriteLine("RaiseResetOnThrow also threw: " + ex2);
                        }
                    }
                    if (AutoDetachThrowingHandlers)
                    {
                        ev -= del;
                        if (TraceDetachThrowingHandlers)
                        {
                            Trace.WriteLine("[error] MultiBindableCollection event handler (NotifyCollectionChangedEventHandler) threw exception.  Detaching.  Exception: " + ex.ToString());
                            //System.Diagnostics.Trace.WriteLine("MultiBindableCollection event handler threw exception.  Detaching.  Exception: " + ex.ToString());
                        }
                    }
                }
            }
        }

        public static void RaiseCollectionChanged<ItemType>(NotifyCollectionChangedHandler<ItemType> ev, NotifyCollectionChangedEventArgs eventArgs)
        {
            // COPY See also: corresponding method in MultiBindableDictionary
            if (ev == null)
            {
                return;
            }

            var args = new NotifyCollectionChangedEventArgs<ItemType>(eventArgs);

#if AOT
			AotSafe.ForEach<object>( ev.GetInvocationList(), delX => {
				NotifyCollectionChangedHandler<ItemType> del = (NotifyCollectionChangedHandler<ItemType>)delX;
#else
            foreach (NotifyCollectionChangedHandler<ItemType> del in ev.GetInvocationList())
            {
#endif

                try
                {
                    IDispatcher dispatcher = del.Target.TryGetDispatcher();

                    if (dispatcher != null && dispatcher.IsInvokeRequired)
                    {
                        dispatcher.BeginInvoke(() => del(args));
                        //dispatcher.BeginInvoke(del, new object[] { args }); // OLD
                        continue;
                    }

#if OLD // WindowsBase
                    if (del.Target as DispatcherObject != null)
                    {
                        DispatcherObject dispatcherObject = (DispatcherObject)del.Target;

                        // REVIEW - Changed to BeginInvoke.  No point in waiting for the invocation to finish! (Unless perhaps we wanted to do error handling, or stuff was happening on a gui thread.) 
                        if (dispatcherObject.Dispatcher != null && !dispatcherObject.Dispatcher.CheckAccess())
                        {
                            dispatcherObject.Dispatcher.BeginInvoke(del, new object[] { args });
#if AOT
								return;
#else
                            continue;
#endif
                        }
                    }
#endif
                    // By default, invoke on same thread                    
                    del.Invoke(args);
                }
                catch (Exception ex)
                {
                    if (AutoDetachThrowingHandlers)
                    {
                        ev -= del;
                        if (TraceDetachThrowingHandlers)
                        {
                            Trace.WriteLine("MultiBindableCollection event handler (NotifyCollectionChangedHandler<T>) threw exception.  Detaching.  Exception: " + ex.ToString());
                            //System.Diagnostics.Trace.WriteLine("MultiBindableCollection event handler threw exception.  Detaching.  Exception: " + ex.ToString());
                        }
                    }
                }
            }
#if AOT
			);
#endif
        }
    }

}
