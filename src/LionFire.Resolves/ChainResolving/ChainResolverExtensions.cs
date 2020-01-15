using LionFire.Dependencies;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Resolves.ChainResolving
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

        public static Task<ChainResolveResult<T>> ChainResolveAsync<T>(this object obj, params object[] parameters)
            => ChainResolveAsync<T>(obj, parameters, null);

        public static async Task<ChainResolveResult<T>> ChainResolveAsync<T>(this object obj, object[] parameters = null, ChainResolveOptions options = null)
        {
            if (options == null) options = DependencyContext.Current.GetService<ChainResolveOptions>() ?? ChainResolveOptions.Default;

            var result = new ChainResolveResult<T>();

            int usedParameters = 0;
            foreach (var resolver in options.AllResolvers)
            {
                if (!options.ContinueResolveCondition(obj)) { return result; }

                Delegate d = resolver.Delegate;
                var methodParameters = d.Method.GetParameters();

                if (!methodParameters[0].ParameterType.IsAssignableFrom(obj.GetType())) continue;

                object[] parametersToUse;
                if (parameters != null)
                {
                    int numParametersToUse = 0;
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
                else
                {
                    parametersToUse = new object[1];
                }

                if (parametersToUse.Length != methodParameters.Length) continue;
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
                    if (options.ReplaceValueCondition(obj, invokeResult))
                    {
                        result.NewSourceValue = invokeResult;
                    }
                    obj = invokeResult;
                }

                if (typeof(T).IsAssignableFrom(result.GetType()))
                {
                    result.IsSuccess = true;
                    break;
                }
            }

            return result;
        }

        //public static ChainResolveResult<T> ChainResolve<T>(this object obj, ChainResolveOptions options = null)
        //{
        //    if (invokeResult is Task<object> task)
        //    {
        //        invokeResult = task.Result; // BLOCKING
        //    }
        //}
    }
}
