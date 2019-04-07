using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Reflection;
using System.IO;

namespace LionFire.Execution.Roslyn.Scripting
{

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
            LionFireEnvironment.MainAppInfo = new AppInfo { ProgramName = "RunnerProgramName" };

            var opts = ScriptOptions.Default
                //.AddReferences("LionFire.Core")
                //.AddReferences("LionFire.Environment")
                  //.AddImports("System")
                  //.AddImports("LionFire")
                  .WithSourceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, AppContext.BaseDirectory));

            Console.WriteLine("ProgramName: " + LionFireEnvironment.ProgramName);

            ScriptState <object> result = null;

            var assembly = typeof(RoslynScriptHost).GetTypeInfo().Assembly;
            string[] names = assembly.GetManifestResourceNames();
            Stream resource = assembly.GetManifestResourceStream("LionFire.Execution.Roslyn.Scripts.TestScript.csx");
            var csharp = new StreamReader(resource).ReadToEnd();

            try
            {
                var script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(csharp, opts);
                result = await script.RunAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine("SCRIPT HOST EXCEPTION: " + ex.ToString());
            }

            if (result?.Exception != null)
            {
                Console.WriteLine("EXCEPTION: " + result.Exception.ToString());
            }

            Console.WriteLine("Return value: " + result?.ReturnValue);
            Console.WriteLine("ProgramName: " + LionFireEnvironment.ProgramName);
            //And this from the REPL
            //Console.WriteLine(CSharpScriptEngine.Execute("new ScriptedClass().HelloWorld"));
            //#endif
            Console.ReadKey();
            return result.ReturnValue;
        }
    }
}
