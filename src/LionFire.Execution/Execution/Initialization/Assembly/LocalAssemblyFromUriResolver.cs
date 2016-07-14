using LionFire.Execution.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if !NET461
using System.Runtime.Loader;
#endif
using System.Threading.Tasks;

namespace LionFire.Execution.Initialization
{
    public enum ConfigurationSelector
    {
        Unspecified = 0,
        Latest,
        Debug,
        Release,
    }

    

    // Search search paths on local disk locations for the assembly.  (Excludes package caches such as github and nuget.)
    public class LocalAssemblyFromUriResolver : IExecutionConfigResolver
    {
        public List<string> LocalDevelopmentTrees = new List<string>
        {
            @"E:\src\",
        };



        // TODO CONFIG
        public string DirForLocalDevelopmentAssembly(string assemblyName, ConfigurationSelector configuration = ConfigurationSelector.Latest)
        {
            var dir = @"E:\src\Trading\src\LionFire.Trading.Feeds.TrueFx\bin\Debug\netstandard1.6\";
            //return dir + assemblyName + ".dll";
            return dir;
        }

        public async Task<bool> Resolve(ExecutionConfig c)
        {
            c.Assembly = await ResolveAssemblyFromName(c.SourceUriBody, c.AssemblyVersion);
            if (c.Assembly != null)
            {
                c.AssemblyName = c.SourceUriBody;
                return true;
            }
            return false;
        }

        public async Task<Assembly> ResolveAssemblyFromName(string assemblyName, string assemblyVersion)
        {
            return await Task.Run(() =>
            {
                Assembly assembly = null;

                var assemblyRef = new AssemblyName(assemblyName);
                if (assemblyVersion != null)
                {
                    assemblyRef.Version = new Version(assemblyVersion);
                }
                try
                {
                    assembly = Assembly.Load(assemblyRef);
                }
                catch { }
                if (assembly != null) { return assembly; }

#if NET461
            throw new NotImplementedException();
#else
                var dir = DirForLocalDevelopmentAssembly(assemblyName);
                var dll = dir + assemblyName + ".dll";
                try
                {
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dll);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception loading assembly: " + dll + System.Environment.NewLine + ex.ToString());
                }
#endif
                return assembly;
            });
        }

    }
}
