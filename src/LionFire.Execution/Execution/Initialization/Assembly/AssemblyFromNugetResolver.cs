using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Execution.Configuration;
using LionFire.ExtensionMethods;

namespace LionFire.Execution.Initialization
{
    public class AssemblyFromNugetResolver : IExecutionConfigResolver
    {
        public string NugetPackageCacheDir = @"E:\Src\Tmp\NugetCache";

        public Task<bool> Resolve(ExecutionConfig config)
        {
            throw new NotImplementedException("nuget support coming soon");
            
//            NugetPackageCacheDir.EnsureDirectoryExists();


//            var psi = new ProcessStartInfo("nuget.exe");
//            psi.WorkingDirectory = NugetPackageCacheDir;
//#if NET462
//            psi.CreateNoWindow = true;
//#endif

//            var p = Process.Start(psi);
        }
    }
}
