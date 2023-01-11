﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LionFire.Structures;
using Microsoft.Extensions.PlatformAbstractions;

namespace LionFire;

public partial class LionFireEnvironment
{


    #region Compile Environment

    /// <summary>
    /// .NET Framework:	NET20, NET35, NET40, NET45, NET451, NET452, NET46, NET461, NET462, NET47, NET471, NET472
    /// .NET Standard:   NETSTANDARD1_0, NETSTANDARD1_1, NETSTANDARD1_2, NETSTANDARD1_3, NETSTANDARD1_4, NETSTANDARD1_5, NETSTANDARD1_6, NETSTANDARD2_0
    /// .NET Core:   NETCOREAPP1_0, NETCOREAPP1_1, NETCOREAPP2_0, NETCOREAPP2_1
    /// - https://docs.microsoft.com/en-us/dotnet/core/tutorials/libraries
    /// </summary>
    public static string CompileTargetFrameworkMoniker =>
#if NET451
                return "NET451";
#elif NET461
                return "net461";
#elif NET462
                return "net462";
#elif NET471
                return "net471";
#elif NET472
                return "net472";
#elif NETCOREAPP1_0
            return "NETCOREAPP1_0";
#elif NETSTANDARD2_0
            "NETSTANDARD2_0";
#elif NETCOREAPP2_0
            return "NETCOREAPP2_0";
#elif NETCOREAPP2_1
            return "NETCOREAPP2_1";
#else
            throw new NotImplementedException("TODO: Implement - https://docs.microsoft.com/en-us/dotnet/core/tutorials/libraries");
#endif

    #region Make DEBUG / TRACE define constants visible to application

    // This only applies to LionFireEnvironment.  FUTURE: try to determine for each DLL somehow
//        public const bool IsDebug =
//#if DEBUG
// true;
//#else
// false;
//#endif

//        public const bool IsTrace =
//#if TRACE
// true;
//#else
// false;
//#endif

    #endregion

    #endregion

    #region Runtime Environment

    //PlatformServices.Default.Application.RuntimeFramework

    public static HashSet<string> UnitTestProducts => unitTestProducts ??= new HashSet<string>
    {
        "Microsoft.TestHost",
        "Microsoft.TestHost.x86",
    };
    private static HashSet<string> unitTestProducts;

    public static bool? IsUnitTest
    {
        get
        {
            if (isUnitTest.HasValue) return isUnitTest.Value;

            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null) return null;

            if (assembly.Modules.Any() && assembly.Modules.First().Name == "testhost.dll") return true;

            // OLD:
            //var entryAssembly = Assembly.GetEntryAssembly();
            //return entryAssembly?.CustomAttributes.OfType<CustomAttributeData>().Where(cad => cad.AttributeType.Name == "AssemblyProductAttribute" && UnitTestProducts.Contains(cad.ConstructorArguments.Select(arg => arg.Value as string).FirstOrDefault())).Any() == true;

            return false;
        }
        set => isUnitTest = value;
    }
    private static bool? isUnitTest;

    // REFACTOR: Merge VosAppHost ExeDir finding logic into here
    public static string ExeDir
    {
        get
        {
#if UNITY
            //				return UnityEngine.Application.dataPath;
            return PersistentDataPath;
#else

#if OLD // Doesn't work with obfuscators
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            if(assembly == null) return "\"";
            
            return System.IO.Path.GetDirectoryName(assembly.Location); 
#else
            var p = Process.GetCurrentProcess();
            var result = p.MainModule.FileName;

            //l.Debug("Process.GetCurrentProcess().MainModule.ModuleName - " + p.MainModule.ModuleName);
            //l.Debug("Process.GetCurrentProcess().MainModule.FileName - " + p.MainModule.FileName);
            //l.Debug("Process.GetCurrentProcess().StartInfo.FileName - " + p.StartInfo.FileName); // Usually empty
            //l.Debug("SEnvironment.CommandLine - " + SEnvironment.CommandLine);

            return System.IO.Path.GetDirectoryName(result);
#endif
#endif
        }
    }

    #endregion

    #region Application(s) Environment

    /// <summary>
    /// True if multiple applications are present within this AppDomain, where an "application" might 
    /// correspond to a Microsoft.Extensions.Hosting host, and ASP.NET application, or a unit test, or a 
    /// scripting engine where each script should be treated as its own application.
    /// If true, statics should be avoided where it would lead to conflicts or improper data sharing between applications.
    /// 
    /// Defaults to true if IsUnitTest is true.
    /// </summary>
    public static bool IsMultiApplicationEnvironment 
    {
        get
        {
            if (isMultiApplicationEnvironment.HasValue) return isMultiApplicationEnvironment.Value;

            return IsUnitTest == true;
        }
        set => isMultiApplicationEnvironment = value;
    }
    private static bool? isMultiApplicationEnvironment;

    #endregion

    /// <summary>
    /// If true, defaults will prefer security over convenience.  Recommended to be true for publically published applications unless you are ok with the implications.
    /// Default: false.
    /// </summary>
    public static bool IsHardenedEnvironment { get; set; }

    #region Streams

    public static TextWriter StandardOutput { get; private set; }

    public static Stream StandardOutputStream
    {
        get
        {
            if (standardOutputStream == null) { StandardOutputStream = Console.OpenStandardOutput(); }
            return standardOutputStream;
        }
        set
        {
            standardOutputStream = value;
            StandardOutput = new StreamWriter(value);
        }
    }
    private static Stream standardOutputStream;

    #endregion

    #region MachineName

    public static string MachineName
    {
        get => machineName ?? System.Environment.MachineName;
        set => machineName = value;
    }

    private static string machineName;

    #endregion

    public static class Compilation
    {
        public static string BuildType { get; set; } = "(unknown)";

        public static bool IsDebug => BuildType == "DEBUG";
        public static bool IsTrace { get; set; }
    }
}
