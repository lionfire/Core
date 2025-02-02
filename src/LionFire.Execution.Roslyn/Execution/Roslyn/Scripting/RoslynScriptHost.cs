using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Reflection;
using System.IO;
using LionFire.Applications;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Hosting;
using System.Runtime.InteropServices;

namespace LionFire.Execution.Roslyn.Scripting;


public class RoslynScriptHost
{

    public string Code { get; set; }

    public async Task Start()
    {
        RoslynScriptContext scriptContext = new RoslynScriptContext();

        var script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(Code, scriptContext.Options);
        var result = await script.RunAsync().ConfigureAwait(false);
    }

    public static async Task<object> TestScript()
    {
        var hab = new HostApplicationBuilder();
        hab.AppInfo(new AppInfo { AppName = "RunnerProgramName" });

        ScriptState<object> result = null;

        await hab.RunAsync(async sp =>
        {
            //ApplicationEnvironment.AppInfo = new AppInfo { AppName  = "RunnerProgramName" };
            var appInfo = sp.GetRequiredService<AppInfo>();


            var opts = ScriptOptions.Default
                  //.AddReferences("LionFire.Core")
                  //.AddReferences("LionFire.Environment")
                  //.AddImports("System")
                  //.AddImports("LionFire")
                  .WithSourceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, AppContext.BaseDirectory));

            Console.WriteLine("ApplicationEnvironment.AppInfo.AppName: " + appInfo.AppName);


            var assembly = typeof(RoslynScriptHost).GetTypeInfo().Assembly;
            string[] names = assembly.GetManifestResourceNames();
            Stream resource = assembly.GetManifestResourceStream("LionFire.Execution.Roslyn.Scripts.TestScript.csx");
            var csharp = new StreamReader(resource).ReadToEnd();

            try
            {
                var script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(csharp, opts);
                result = await script.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SCRIPT HOST EXCEPTION: " + ex.ToString());
            }

            if (result?.Exception != null)
            {
                Console.WriteLine("EXCEPTION: " + result.Exception.ToString());
            }

            Console.WriteLine("Return value: " + result?.ReturnValue);
            Console.WriteLine("ApplicationEnvironment.AppInfo.AppName: " + appInfo.AppName);
            //And this from the REPL
            //Console.WriteLine(CSharpScriptEngine.Execute("new ScriptedClass().HelloWorld"));
            //#endif
            Console.ReadKey();
        });
        return result.ReturnValue;
    }
}
