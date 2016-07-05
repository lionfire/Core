using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LionFire.Scripting.Roslyn
{
    public class RoslynScriptHost
    {
        public static void TestScript()
            {
#if NET461
            CSharpScriptEngine.Execute(
            //This could be code submitted from the editor
            @"
            public class ScriptedClass
            {
                public String HelloWorld {get;set;}
                public ScriptedClass()
                {
                    HelloWorld = ""Hello Roslyn!"";
                }
            }");
            //And this from the REPL
            Console.WriteLine(CSharpScriptEngine.Execute("new ScriptedClass().HelloWorld"));
#endif
            Console.ReadKey();
        }
    }
}
