using Microsoft.Extensions.Logging;
using System;
using System.Windows.Threading;

namespace LionFire.Bindings.Windows
{
    public static class LionBindingNodeDispatcherExtensions
    {
        public static void Init()
        {
            LionBindingNode.TryUpdateAccessorWrapperMethods = _TryUpdateAccessorWrapperMethods;
        }

        public static (bool succeeded, Func<LionBindingNode, object ,object>,Action<LionBindingNode, object, object>) _TryUpdateAccessorWrapperMethods(object BindingObject)
        {
            DispatcherObject depObj = BindingObject as DispatcherObject;
            if (depObj != null)
            {
                //GetMethodWrapper = GetWrapperForDispatcherObject;
                //SetMethodWrapper = SetWrapperForDispatcherObject;
                return (true, GetWrapperForDispatcherObject, SetWrapperForDispatcherObject);
            }
            return (false,null,null);
        }

        public static object GetWrapperForDispatcherObject(this LionBindingNode node, object o)
        {
            DispatcherObject depObj = o as DispatcherObject;
            if (depObj != null && !depObj.Dispatcher.CheckAccess())
            {
                return depObj.Dispatcher.Invoke(DispatcherPriority.Normal, new Func<object>(() =>
                // return depObj.Dispatcher.Invoke(new Func<object>(() =>
                { return node.GetMethod(o); }
                      ));
            }
            else
            {
                return node.GetMethod(o);
            }
        }
        public static void SetWrapperForDispatcherObject(this LionBindingNode node, object o, object value)
        {
            DispatcherObject depObj = o as DispatcherObject;
            if (depObj != null && !depObj.Dispatcher.CheckAccess())
            {
                depObj.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                //                depObj.Dispatcher.Invoke(new Action(() =>  // BACKPORTED
                {
                    try
                    {
                        node.SetMethod(o, value);
                    }
                    catch (Exception ex)
                    {
                        l.LogError(node.ToString() + ": SetMethod threw exception: " + ex);
                    }
                }
                      ));
            }
            else
            {
                try
                {
                    node.SetMethod(o, value);
                }
                catch (Exception ex)
                {
                    l.LogError(node.ToString() + ": SetMethod threw exception: " + ex);
                }
            }
        }

        public static ILogger l = Log.Get();
    }
}
