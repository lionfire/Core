using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LionFire.CommandLine.Arguments;

namespace LionFire.CommandLine.Dispatching
{
    
    public static class CliDispatcher
    {
        static readonly CliDispatcherOptions DefaultOptions = new CliDispatcherOptions();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="context"></param>
        /// <param name="handlerClass">If null, type of context will be used</param>
        /// <returns>True if something was dispatched, false otherwise (and options.Usage method was invoked if present)</returns>
        public static bool Dispatch(this string[] args, object handlerInstance = null, Type handlerClass = null, object context = null, CliDispatcherOptions options = null)
        {
            if (options == null) options = DefaultOptions;
            var argDefs = CliArgumentDefinition.GetForType(handlerInstance?.GetType() ?? handlerClass ?? handlerInstance.GetType());

#region AdditionalHandlerTypes

            if (options.AdditionalHandlerTypes != null)
            {
                foreach (var additionalTypeName in options.AdditionalHandlerTypes)
                {
                    Type additionalType = null;
                    try
                    {
                        additionalType = Type.GetType(additionalTypeName);
                    }
                    catch
                    {
Console.WriteLine("TEMP - Failed to load class " + additionalTypeName);
                    }
                    if (additionalType != null)
                    {
                        var additionalDefinitions = CliArgumentDefinition.GetForType(type: additionalType);
                        // TODO: only add ones that don't overlap
                        argDefs.AddRange(additionalDefinitions);
                    }
                }
            }

#endregion

            argDefs.Validate();

            var cla = args.ParseCommandLineArgs();

            if (cla.Args.Count <= 0)
            {
                if (options.DefaultUsageLongForm != null)
                {
                    var argDef = argDefs.Where(def => def.LongForm == options.DefaultUsageLongForm).FirstOrDefault();
                    if (argDef != null)
                    {
                        RunHandler(argDef.Handler, handlerInstance ?? argDef.HandlerInstance, context, args, cla);
                    }
                }
                return false;
            }
            return true;
        }

        private static void RunHandler(MethodInfo handler, object instance, object context, string[] args, CommandLineArgs cla)
        {
            var paraDefs = handler.GetParameters();

            if (paraDefs.Length == 0)
            {
                handler.Invoke(instance, null);
            }

            List<object> paras = new List<object>();
            foreach (var pi in paraDefs)
            {
                if (pi.ParameterType == typeof(string[]))
                {
                    paras.Add(args);
                }
                else if (pi.ParameterType == typeof(object))
                {
                    paras.Add(context);
                }
                else if (pi.ParameterType == typeof(CommandLineArgs))
                {
                    paras.Add(cla);
                }
            }

            handler.Invoke(instance, paras.ToArray());
        }
    }

}
