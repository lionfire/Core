#if NET4
//#define PARALLEL_INIT
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
//using LionFire.Assemblies;

namespace LionFire.Applications
{
    /// <summary>
    /// Finds all types in all assemblies with the IAssemblyInitializer interface,
    /// instantiate it and invoke Initialize.
    /// </summary>
    public class AssemblyInitializer
    {
        public List<string> IgnoredAssembliesBySubstring = new List<string>();

        public bool InitializeAllAssemblies;
        public bool AllowMultipleAssemblyInitialization = false;
        public bool AsyncAssemblyInit = false;

        public bool IsInitialized
        {
            get
            {
                lock (isInitializedLock)
                {
                    return isInitialized;
                }
            }
            private set
            {
                lock (isInitializedLock)
                {
                    if (isInitialized == value) return;

                    isInitialized = value;

                    if (isInitialized)
                    {
                        FlushInitQueue();
                        assembliesToInitialize = null;
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
                if (isInitialized)
                {
                    FlushInitQueue();
                }
            }
        }
        private bool isInitialized;
        private object isInitializedLock = new object();

        private void Enqueue(Assembly a) // REVIEW ThreadSafety
        {
            if (assembliesToInitialize == null)
            {
                lock (isInitializedLock)
                {
                    assembliesToInitialize = new ConcurrentQueue<Assembly>();
                    isInitialized = false;
                }
            }

            assembliesToInitialize.Enqueue(a);
        }
        private ConcurrentQueue<Assembly> assembliesToInitialize = new ConcurrentQueue<Assembly>();
        private ConcurrentDictionary<string, Assembly> initializedAssemblies = new ConcurrentDictionary<string, Assembly>();

        public AssemblyInitializer()
        {
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);

            // TODO: Regular expression support
            //AssemblyInitializer.IgnoredAssembliesBySubstring.Add("Microsoft.");
        }

        void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (!args.LoadedAssembly.ReflectionOnly)
            {
                if (AsyncAssemblyInit)
                {
                    Task.Factory.StartNew(() => // Do the init before the assembly can be used?
                        { _InitLoadedAssembly(args.LoadedAssembly); });
                }
                else
                {
                    _InitLoadedAssembly(args.LoadedAssembly);
                }
            }
        }

        private void _InitLoadedAssembly(Assembly assembly)
        {
            if (Monitor.TryEnter(isInitializedLock))
            {
                try
                {
                    if (IsInitialized)
                    {
                        Initialize(assembly);
                    }
                    else
                    {
                        Enqueue(assembly);
                    }
                }
                finally
                {
                    Monitor.Exit(isInitializedLock);
                }
            }
            else
            {
                Enqueue(assembly);
            }

        }

        private void FlushInitQueue()
        {
            Assembly assembly;
            while (assembliesToInitialize != null && assembliesToInitialize.TryDequeue(out assembly))
            {
                Initialize(assembly);
            }
        }

        public void Initialize()
        {
            IsInitialized = true;

            foreach (Assembly assembly in
#if AOT
			         (IEnumerable)
#endif
 AppDomain.CurrentDomain.GetAssemblies()
#if PARALLEL_INIT
.AsParallel()
#endif
)
            {
                try
                {
                    //LionFireApp.Current.SplashMessage = "Initializing " + assembly.FullName;
                    Initialize(assembly);
                }
                catch (Exception ex)
                {
                    l.Error("Exception initializing assembly '" + assembly.FullName + "': " + ex.ToString());
                }
                finally
                {
                    //LionFireApp.Current.SplashMessage = "Initialized " + assembly.FullName;
                }
            }
        }

     

        public void Initialize(Assembly assembly)
        {
            string name = assembly.FullName;


            if (assembly.FullName.Contains("Linux") )
            {
                throw new NotImplementedException("TODO: Linux assemblies");
#if TODO
                if (LionFireEnvironment.Platform != Platform.Linux)
                {
                    l.Debug("Skipping assembly because it is for Linux: " + assembly.FullName);
                    return;
                }
#endif
            }

#if TOPORT // TODO
            var attr = assembly.GetCustomAttribute<AssemblyPlatformsAttribute>();
            if(attr!=null)
            {
                if ((attr.Platform & LionEnvironment.Platform) !=LionEnvironment.Platform)
                {
                    l.Debug("Skipping assembly because it is only for other platforms: " + attr.Platform + " - " + assembly.FullName);
                    return;
                }
            }
#endif

            foreach (string substring in
#if AOT
			         (IEnumerable)
#endif
 IgnoredAssembliesBySubstring)
            {
                if (assembly.FullName.Contains(substring))
                {
                    l.Trace("Ignoring assembly based on ignore rule: " + assembly.FullName);
                    return;
                }
            }

            bool isNew = initializedAssemblies.TryAdd(assembly.FullName, assembly);

            if (!isNew && !AllowMultipleAssemblyInitialization) return;

            UtilityAssemblyInitializer.Initialize();

            bool foundInitializer = false;

            Type staticType = null;
            try
            {
                staticType = assembly.GetType(assembly.FullName + "AssemblyInitializer");
                if (staticType != null)
                {
                    foundInitializer = true;
                    var initializer = (IAssemblyInitializer)Activator.CreateInstance(staticType);
                    initializer.Initialize();
                }
            }
            catch(Exception ex)
            {
                l.Error("Builder " + (staticType == null ? "(null)" : staticType.FullName) + " threw exception: " + ex.ToString());
            }

            if (!foundInitializer)
            {
                // Scan for IAssemblyInitializer types:  (DEPRECATED: Use hardcoded path
                try
                {


                    foreach (Type initializerType in
#if AOT
			         (IEnumerable)
#endif
 assembly.GetTypes()
#if PARALLEL_INIT && !MONO
.AsParallel()
#endif
.Where(t => typeof(IAssemblyInitializer).IsAssignableFrom(t)))
                    {
                        if (initializerType.IsInterface || initializerType.IsAbstract) continue;
                        try
                        {
                            l.LogWarning("TODO: Set splash message to Initializing " + initializerType.FullName);
#if TODO
                            LionFireApp.Current.SplashMessage = "Initializing " + initializerType.FullName;
#endif
                            var initializer = (IAssemblyInitializer)Activator.CreateInstance(initializerType);
                            initializer.Initialize();
                            l.Trace("Builder '" + initializerType.FullName + "' executed.");
                        }
                        catch (Exception ex)
                        {
                            l.Error("Builder '" + initializerType.FullName + "' threw exception: " + ex.ToString());
                        }
                        finally
                        {
                            l.LogWarning("TODO: Set splash message to Initialized " + initializerType.FullName);
#if TODO
                            LionFireApp.Current.SplashMessage = "Initialized " + initializerType.FullName;
#endif
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = "[ASSEMBLY INIT] " + assembly.FullName + " threw exception when scanning for IAssemblyInitializer types: " + ex.ToString();

                    ReflectionTypeLoadException rtle = ex as ReflectionTypeLoadException;
                    if (rtle != null)
                    {
                        foreach (var lex in rtle.LoaderExceptions)
                        {
                            msg += Environment.NewLine + " ------- Loader Exception: ----- " + Environment.NewLine + lex;
                        }
                    }

                    l.Error(msg);

                    Assembly o;
                    initializedAssemblies.TryRemove(assembly.FullName, out o);
                }
            }
        }

        private static readonly ILogger l = Log.Get();
    }

    public interface IAssemblyInitializer
    {
        void Initialize();
    }

    public class UtilityAssemblyInitializer
    {
        public static void Initialize()
        {
            if (isInitialized) return;

#if fastJSON
#if !UNITY
            fastJSON.JSON.Parameters.IgnoreAttributes = new List<Type> { typeof(IgnoreAttribute) };
#endif
#endif
            isInitialized = true;
        }
        private static bool isInitialized = false;
    }

}
