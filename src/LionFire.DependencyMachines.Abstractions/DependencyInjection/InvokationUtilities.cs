#nullable enable
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.DependencyInjection
{
    public static class InvokationUtilities
    {
        public static object Invoke(IServiceProvider serviceProvider, Delegate del)
        {
            var mi = del.GetType().GetMethod("Invoke");
            var p = mi.GetParameters();
            var parameters = new object[p.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (p[i].DefaultValue != null)
                {
                    parameters[i] = serviceProvider.GetService(p[i].ParameterType) ?? p[i].DefaultValue;
                }
                else
                {
                    parameters[i] = serviceProvider.GetRequiredService(p[i].ParameterType);
                }
            }
            return mi.Invoke(null, parameters);
        }

        public static (Type? missingDependency, object? result) TryInvoke(IServiceProvider serviceProvider, Delegate del)
        {
            var mi = del.GetType().GetMethod("Invoke");
            var p = mi.GetParameters();
            var parameters = new object[p.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                if (p[i].DefaultValue != null)
                {
                    parameters[i] = serviceProvider.GetService(p[i].ParameterType) ?? p[i].DefaultValue;
                }
                else
                {
                    parameters[i] = serviceProvider.GetService(p[i].ParameterType);
                    if (parameters[i] == null)
                    {
                        return (p[i].ParameterType, null);
                    }
                }
            }
            return (null, mi.Invoke(null, parameters));
        }
    }
}
