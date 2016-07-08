using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if NET461
#else
using System.Runtime.Loader;
#endif
using System.Threading.Tasks;

namespace LionFire.Services.Hosting
{

    // TODO:
    //  - restart/fallback options, 
    //  - error feedback? notify on failure

    public class ServiceConfig
    {
        public string Key { get { return ServiceType?.FullName ?? ServiceTypeName; } } // TODO: Make unique, maybe a GUID
        public Guid Guid { get; set; }

        public string Arg {
            get { return arg; }
            set {
                this.arg = value;
            }
        }
        private string arg;

#region Construction

        public ServiceConfig() { }
        public ServiceConfig(string arg) { this.Arg = arg; }

        //public ServiceConfig(string package, string type, string parameter = null) { }

#endregion

#region Service Reference

        public string PackageSchema { get; set; }

        public string Package { get; set; }

        public string ServiceTypeName { get; set; }

        public Type ServiceType { get; set; }

#endregion

#region Hosting Configuration
        public ServiceConfigFlags Flags { get; set; }

#endregion

#region Service Parameters

        public Dictionary<string, object> Parameters { get; set; }

#endregion

#region Runtime Location


        public string Hive { get; set; }

        /// <summary>
        /// If null, the configuraton for the ServiceHost configuration will be used.
        /// </summary>
        public string Host { get; set; }

        public string ProcessName { get; set; }

        public HostingLocationType HostingLocationType { get; internal set; } = HostingLocationType.InProcess;

#endregion

#region Runtime Parameters

        public string Platform { get; set; } = "NetCoreApp1.0"; // MOVE Default hardcoded string

#endregion

#region Identifier Parameters

        string assemblyName = null;
        string assemblyVersion = null;


#endregion

        public List<string> LocalDevelopmentTrees = new List<string>
        {
            @"E:\src\",
        };

        public enum ConfigurationSelector
        {
            Unspecified = 0,
            Latest,
            Debug,
            Release,
        }

        public string DirForLocalDevelopmentAssembly(string assemblyName, ConfigurationSelector configuration = ConfigurationSelector.Latest)
        {
            var dir = @"E:\src\Trading\src\LionFire.Trading.Feeds.TrueFx\bin\Debug\netstandard1.6\";
            //return dir + assemblyName + ".dll";
            return dir;
        }

        public async Task<Assembly> ResolveAssembly(string assemblyName, string assemblyVersion)
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


        public async Task<bool> TryResolveServiceType(bool reresolve = false)
        {
            if (!reresolve && this.ServiceType != null) return true;

            Package = null;
            PackageSchema = null;
            assemblyName = null;
            assemblyVersion = null;
            ServiceTypeName = null;
            ServiceType = null;

            string typeName = this.Arg;

            if (typeName.Contains("]"))
            {
                var split = this.Arg.Split(new char[] { ']' }, 2);
                Package = split[0].TrimStart('[');
                typeName = split[1];
            }

            if (Package != null && Package.Contains(":"))
            {
                var split = this.Arg.Split(new char[] { ':' }, 2);
                PackageSchema = split[0].TrimStart('[');
                Package = split[1];
            }

            //char AssemblyTypeSeparator = ';';
            char AssemblyTypeSeparator = '/';

            if (typeName.Contains(AssemblyTypeSeparator.ToString()))
            {
                var split = this.Arg.Split(new char[] { AssemblyTypeSeparator }, 2);
                assemblyName = split[0];
                typeName = split[1];
            }

            if (assemblyName.Contains("`"))
            {
                var split = this.Arg.Split(new char[] { '`' }, 2);
                assemblyName = split[0];
                assemblyVersion = split[1];
            }

            Assembly assembly = null;
            if (assemblyName != null)
            {
                assembly = await ResolveAssembly(assemblyName, assemblyVersion);

                if (assembly == null)
                {
                    throw new Exception("Failed to load assembly: " + assemblyName + " " + assemblyVersion);
                }
            }

            if (assembly != null)
            {
                Type type = assembly.GetType(typeName);
                if (type == null)
                {
                    type = assembly.GetType(assemblyName + "." + typeName);
                }
                this.ServiceType = type;
            }
            else
            {
                Type type = Type.GetType(typeName);
                this.ServiceType = type;
            }

            this.ServiceTypeName = typeName;

            return this.ServiceType != null;
        }

    }


}
