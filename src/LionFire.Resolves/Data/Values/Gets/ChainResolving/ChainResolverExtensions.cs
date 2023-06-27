using LionFire.Dependencies;
using LionFire.Structures;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets.ChainResolving
{
    /// <summary>
    /// Iteratively resolve an object until it is the expected type
    /// Potential subsystems:
    ///  - string to IReference 
    ///  - ITemplate to template instance
    ///  - color code to framework-specific color (CSS, WPF, WinForms, etc.)
    ///  - All of the above:   "vos:/palettes/default", "/dark/primary"
    /// E.g.:
    ///   - IVob vob = "vos:/path/to/vob"
    /// </summary>
    public static class ChainResolverExtensions
    {

        // TODO: Two different versions of ResolveTo that do in-place replace or not.  But should it be encouraged to configure this on the producer end?
        //public static TValue ResolveTo<TValue>(this IWrapper<object> hasObject, ChainResolveOptions options = null, params object[] parameters)
        //{
        //}

        public static T ResolveTo<T>(this IWrapper<object> hasObject, ChainResolveOptions options = null, params object[] parameters)
        {
            var result = hasObject.Value.ChainResolveAsync<T>(options, parameters).Result;
            if (result.HasNewSourceValue)
            {
                hasObject.Value = result.NewSourceValue;
            }
            return result.ResolvedValue;
        }

        //public static Task<ChainResolveResult<TValue>> ChainResolveAsync<TValue>(this object obj, params object[] parameters)
        //    => ChainResolveAsync<TValue>(obj, parameters, null);

        public static ChainResolveOptions CurrentOptions => DependencyContext.Current.GetService<IOptionsMonitor<ChainResolveOptions>>()?.CurrentValue
                ?? ChainResolveOptions.Default;

        public static async Task<ChainResolveResult<T>> ChainResolveAsync<T>(this object obj, ChainResolveOptions options = null, params object[] parameters)
        {
            if (options == null) options = CurrentOptions;

            var result = new ChainResolveResult<T>();

            int usedParameters = 0;
            foreach (var resolver in options.AllResolvers)
            {
                if (options.ContinueResolveCondition?.Invoke(obj) == false) { return result; }

                Delegate d = resolver.Delegate;
                var methodParameters = d.Method.GetParameters();

                if (!methodParameters[0].ParameterType.IsAssignableFrom(obj.GetType())) continue;

                object[] parametersToUse;
                int numParametersToUse = 0;
                if (parameters != null && parameters.Length > 0)
                {
                    for (int methodIndex = 1, parametersIndex = usedParameters; (1 + methodIndex) < methodParameters.Length &&
                        parametersIndex < parameters.Length; methodIndex++, parametersIndex++)
                    {
                        if (methodParameters[methodIndex].ParameterType.IsAssignableFrom(parameters[parametersIndex]?.GetType()))
                        {
                            numParametersToUse++;
                        }
                        else numParametersToUse = -1;
                    }

                    if (numParametersToUse == -1) continue;
                    usedParameters += numParametersToUse;


                    if (numParametersToUse == parameters.Length)
                    {
                        parametersToUse = parameters;
                    }
                    else
                    {
                        parametersToUse = new object[numParametersToUse + 1];
                        Array.Copy(parameters, usedParameters - numParametersToUse, parametersToUse, 1, numParametersToUse);
                    }
                }

                if (numParametersToUse + 1 != methodParameters.Length) continue;
                parametersToUse = new object[numParametersToUse + 1];
                parametersToUse[0] = obj;
                var invokeResult = resolver.Delegate.DynamicInvoke(parametersToUse);

                while (invokeResult is Task<object> task) { invokeResult = await task; }

                if (invokeResult is ChainResolveResult<T> crr)
                {
                    if (crr.HasNewSourceValue)
                    {
                        if (options.ReplaceValueCondition(obj, crr.NewSourceValue))
                        {
                            result.NewSourceValue = crr.NewSourceValue;
                        }
                    }
                    if (crr.IsSuccess)
                    {
                        obj = crr.ResolvedValue;
                    }
                }
                else
                {
                    if (options.ReplaceValueCondition?.Invoke(obj, invokeResult) != false)
                    {
                        result.NewSourceValue = invokeResult;
                        result.HasNewSourceValue = true;
                    }
                    obj = invokeResult;
                }

                if (typeof(T).IsAssignableFrom(obj?.GetType()))
                {
                    result.ResolvedValue = (T)obj;
                    result.IsSuccess = true;
                    break;
                }
            }

            return result;
        }

        //public static ChainResolveResult<TValue> ChainResolve<TValue>(this object obj, ChainResolveOptions options = null)
        //{
        //    if (invokeResult is Task<object> task)
        //    {
        //        invokeResult = task.Result; // BLOCKING
        //    }
        //}
    }
}
