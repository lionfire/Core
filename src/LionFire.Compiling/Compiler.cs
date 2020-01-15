using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;

namespace LionFire.Compiling
{
    public class RunOptions
    {
        public string EntryType { get; set; }
        public string EntryMethod { get; set; }
        public bool IsCollectible { get; set; }
        public bool AutoCollect { get; set; }

    }        

    public class RunResult
    {
        public object ReturnValue { get; set; }
        public AssemblyLoadContext AssemblyLoadContext { get; set; }
    }
    public class CollectibleAssemblyLoadContext : AssemblyLoadContext
    {
        public CollectibleAssemblyLoadContext() : base(isCollectible: true) { }
    }
    public class NoncollectibleAssemblyLoadContext : AssemblyLoadContext
    {
        public NoncollectibleAssemblyLoadContext() : base(isCollectible: false) { }
    }

    public class Compiler
    {
        private static Assembly SystemRuntime = Assembly.Load(new AssemblyName("System.Runtime"));

        string NewAssemblyId => Guid.NewGuid().ToString().Replace("-", "");

        public CSharpCompilation Compile(string code, CompilerOptions options = null)
        {
            //return CSharpCompilation.Create("DynamicAssembly_" + NewAssemblyId, new[] { CSharpSyntaxTree.ParseText(code) },
            return CSharpCompilation.Create("DynamicAssembly", new[] { CSharpSyntaxTree.ParseText(code) },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(SystemRuntime.Location),
            },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public RunResult Run(CSharpCompilation compilation, string entryType, string entryMethod, bool collectible = true, bool autoUnload = true, object[] parameters = null)
        {
            var context = collectible ? new CollectibleAssemblyLoadContext() : (AssemblyLoadContext)new NoncollectibleAssemblyLoadContext();

            object result;
            using (var ms = new MemoryStream())
            {
                var cr = compilation.Emit(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var assembly = context.LoadFromStream(ms);

                var type = assembly.GetType(entryType);
                var greetMethod = type.GetMethod(entryMethod);

                var instance = Activator.CreateInstance(type);
                result = greetMethod.Invoke(instance, parameters);
            }
            if(collectible && autoUnload) context.Unload();
            return new RunResult { AssemblyLoadContext = context, ReturnValue = result };
        }
    }
}
