#nullable enable
using LionFire.Dependencies;
using LionFire.Instantiating;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace LionFire.DependencyInjection
{
    public static class InvokationUtilities
    {
        public static object? Invoke(IServiceProvider serviceProvider, Delegate del, params (Type, Func<IServiceProvider, object>)[] additionalServices)
        {
            var result = TryInvoke(serviceProvider, del, additionalServices);
            if (result.missingDependency != null)
            {
                throw new DependencyMissingException(result.missingDependency.ToString());
            }
            return result.result;
        }

        public static (Type? missingDependency, object? result) TryInvoke(IServiceProvider serviceProvider, Delegate del, params (Type, Func<IServiceProvider, object>)[] additionalServices)
        {
            var mi = del.GetType().GetMethod("Invoke");
            var p = mi.GetParameters();
            var parameters = new object[p.Length];

            Dictionary<Type, Func<IServiceProvider, object>>? dict = null;
            if (additionalServices.Length > 0)
            {
                dict = new Dictionary<Type, Func<IServiceProvider, object>> ();
                foreach (var item in additionalServices)
                {
                    dict.Add(item.Item1, item.Item2);
                }
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                Type parameterType = p[i].ParameterType;
                parameters[i] = dict?[parameterType]?.Invoke(serviceProvider) ?? serviceProvider.GetService(parameterType) ?? p[i].DefaultValue;
                if (parameters[i] == null) { return (parameterType, null); }
            }
            return (null, mi.Invoke(null, parameters));
        }
    }
}
