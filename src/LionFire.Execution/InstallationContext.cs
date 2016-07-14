using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace LionFire.Execution
{
    //public interface IExecutionState
    //{
    //    bool IsRunning { get; set; }

    //    Task<bool> Stop(CancellationTokenSource cts = null);

    //    bool CanStop { get; }

    //    Task<bool> Pause(int millisecondsTimeout);

    //    bool CanPause { get; }

    //}

    //public class ExecutionState
    //{

    //    //void a()
    //    //{
    //    //    Task<bool> a;
    //    //    a.CreationOptions = TaskCreationOptions.
    //    //}

    //}

    //public class ObjectExecutionState
    //{
    //    public object ExecutionObject { get; set; }
    //}

    public enum InstallationKind
    {
        Unspecified,
        Temporary,
        LocalUser,
        LocalMachine,
        LocalNetwork
    }



    public class InstallationContext
    {

        /// <summary>
        /// The Uri that summarizes the request for execution.  
        /// Examples:
        ///  - gist:lionfire/SampleScript.csscript.cs
        ///  - gist:lionfire/SampleScript.roslyn.cs
        ///  - nuget:LionFire.Services.Samples/SampleService
        /// </summary>
        public string InstallationUri { get; set; }

        /// <summary>
        /// The physical location to which the installation is installed
        /// </summary>
        public string InstallationTargetUri { get; set; }


    }

    public class AssemblyResolverConfig
    {
        public string Uri { get; set; }

        public static implicit operator AssemblyResolverConfig(string uri) { return new AssemblyResolverConfig { Uri = uri }; }
    }

    public class AssemblyResolvingConfig
    {
        public SortedDictionary<int, AssemblyResolverConfig> Resolvers = new SortedDictionary<int, AssemblyResolverConfig>
        {
            [5] = "file://e:/src/**",
            [10] = "nuget:https://nuget.lionfire.ca",
            [10] = "nuget:nuget.org",
            [50] = "nuget",
            [52] = "myget",
            [60] = "choco",
        };
    }


    [Flags]
    public enum ExecutionInstallationFlags
    {
        None,
        ShadowCopy = 1 << 0,
    }

    
}
