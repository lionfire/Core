using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace LionFire.Execution.Roslyn.Scripting
{
    
    public class RoslynScriptHost
    {

        public string Code { get; set; }

        public async Task Start()
        {
            RoslynScriptContext scriptContext = new RoslynScriptContext();


            var script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(Code
            //CSharpScriptEngine.Execute(
            , scriptContext.Options);
            var result = await script.RunAsync();
            //And this from the REPL
            //Console.WriteLine(CSharpScriptEngine.Execute("new ScriptedClass().HelloWorld"));
            //Console.ReadKey();

        }


        public static void TestScript()
        {
            var opts = ScriptOptions.Default.
                  AddImports("System").
                  WithSourceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, AppContext.BaseDirectory));


            //#if NET462
            var script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(
            //CSharpScriptEngine.Execute(
            //This could be code submitted from the editor
            @"
            public class ScriptedClass
            {
                public String HelloWorld {get;set;}
                public ScriptedClass()
                {
                    HelloWorld = ""Hello Roslyn!"";
                    Console.WriteLine(""Hello from inside script"");
                }
            }
new ScriptedClass();
", opts);
            script.RunAsync().Wait();
            //And this from the REPL
            //Console.WriteLine(CSharpScriptEngine.Execute("new ScriptedClass().HelloWorld"));
            //#endif
            Console.ReadKey();
        }
    }
}
