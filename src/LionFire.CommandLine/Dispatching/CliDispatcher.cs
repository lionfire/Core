using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LionFire.CommandLine.Arguments;
using System.Threading;

namespace LionFire.CommandLine.Dispatching
{
    public class CliDispatcher
    {
        public static CliDispatcherOptions DefaultOptions = new CliDispatcherOptions();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="context"></param>
        /// <param name="handlerClass">If null, type of context will be used</param>
        /// <returns>True if something was dispatched, false otherwise (and options.Usage method was invoked if present)</returns>
        public static bool Dispatch(string[] args, object handlerInstance = null, Type handlerClass = null, object context = null, CliDispatcherOptions options = null, CancellationToken? cancellationToken = null)
        {
            //CliDispatcher cd = new CliDispatcher(); // FUTURE

            if (options == null) options = CliDispatcher.DefaultOptions;

            List<CliArgumentDefinition> argDefs;
            if (handlerInstance != null)
            {
                argDefs = CliArgumentDefinition.GetForType(handlerInstance);
            }
            else
            {
                argDefs = CliArgumentDefinition.GetForType(type: handlerClass);
            }

            #region AdditionalHandlerTypes

            if (options.AdditionalHandlerTypeNames != null)
            {
                foreach (var additionalTypeName in options.AdditionalHandlerTypeNames)
                {
                    Type additionalType = null;
                    try
                    {
                        additionalType = Type.GetType(additionalTypeName);
                    }
                    catch
                    {
                    }
                    if (additionalType != null)
                    {
                        var additionalDefinitions = CliArgumentDefinition.GetForType(type: additionalType);
                        // TODO: only add ones that don't overlap
                        argDefs.AddRange(additionalDefinitions);
                    }
                    else
                    {
                        Console.WriteLine("TEMP - Failed to load class " + additionalTypeName);
                        Console.WriteLine("test: " + typeof(LionFire.CommandLine.CommandLineArgs).AssemblyQualifiedName);
                    }
                }
                foreach (var additionalType in options.AdditionalHandlerTypes)
                {
                    var additionalDefinitions = CliArgumentDefinition.GetForType(type: additionalType);
                    // TODO: only add ones that don't overlap
                    argDefs.AddRange(additionalDefinitions);
                }
            }

            #endregion

            argDefs.Validate();

            var cla = args.ParseCommandLineArgs();

            bool showUsage = true;

            if (cla.Args.Count >= 1)
            {
                var argDef = argDefs.Where(def => def.LongForm == cla.Verb || def.ShortForm.ToString() == cla.Verb).FirstOrDefault();
                if (argDef != null)
                {
                    showUsage = false;
                    RunHandler(argDef.Handler, handlerInstance ?? argDef.HandlerInstance, context, args, cla);
                }
            }

            if (showUsage)
            {
                if (options.DefaultUsageLongForm != null)
                {
                    var argDef = argDefs.Where(def => def.LongForm == options.DefaultUsageLongForm).FirstOrDefault();
                    if (argDef != null)
                    {
                        RunHandler(argDef.Handler, handlerInstance ?? argDef.HandlerInstance, context, args, cla, cancellationToken);
                    }
                    else
                    {
                        Console.WriteLine("Unrecognized command line arguments.  Please consult documentation.");
                    }
                }
                return false;
            }

            return true;
        }

        private static void RunHandler(MethodInfo handler, object instance, object context, string[] args, CommandLineArgs cla, CancellationToken? cancellationToken = null)
        {
            var paraDefs = handler.GetParameters();

            if (paraDefs.Length == 0)
            {
                handler.Invoke(instance, null);
                return;
            }

            List<object> paras = new List<object>();
            foreach (var pi in paraDefs)
            {
                if (pi.ParameterType == typeof(string[]))
                {
                    paras.Add(args.Skip(1).ToArray());
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

            Invoker.Invoke(handler, instance, paras.ToArray());
            //handler.Invoke(instance, paras.ToArray());
        }

        public static Func<MethodInfo, object, object[], object> Invoker { get; set; } = (handler, instance, parameters) => handler.Invoke(instance, parameters);
    }

    public static class CliDispatcherExtensions
    {
        public static bool Dispatch(this string[] args, object handlerInstance = null, Type handlerClass = null, object context = null, CliDispatcherOptions options = null)
        {
            return CliDispatcher.Dispatch(args, handlerInstance, handlerClass, context, options);
        }
    }

}
