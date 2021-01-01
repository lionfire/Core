#if OverlayProxies
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace LionFire.Overlays
{
    public class OverlayInterceptor : IInterceptor
    {
        private static readonly ILogger l = Log.Get();

        internal PropertyInfo GetProperty(Type type, string methodName)
        {
            return type.GetProperty(methodName.Substring(4));
        }

        public void Intercept(IInvocation invocation)
        {
            string methodName = invocation.Method.Name;

            IOverlay overlay = (IOverlay)invocation.Proxy;

            PropertyInfo pi = null;
            if (methodName.StartsWith("set_"))
            {
                string propertyName = methodName.Substring(4);

                overlay.SetPropertyValue(propertyName, invocation.Arguments[0], (object[])invocation.Arguments[1]);
                return;
            }
            else if (methodName.StartsWith("get_"))
            {
                string propertyName = methodName.Substring(4);

                invocation.ReturnValue = overlay.GetPropertyValue(propertyName, invocation.Arguments);
                return;
            }

            if (pi == null)
            {
                l.Warn("UNTESTED/UNEXPECTED: Proceeding with non-property method for overlay: " + methodName);
                invocation.Proceed();
                return;
            }

            invocation.Proceed();
        }
    }
}
#endif
