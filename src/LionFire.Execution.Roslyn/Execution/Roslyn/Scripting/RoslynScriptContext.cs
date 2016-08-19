#define GLOBALS

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection.Metadata;

namespace LionFire.Execution.Roslyn.Scripting
{
    public class RoslynScriptContext
    {
        public string ScriptsDirectory { get { return AppContext.BaseDirectory; } }

        public List<string> SearchPaths { get; set; }

        public List<string> Imports { get; set; }


        public ScriptOptions Options {
            get {

                var options = ScriptOptions.Default
                    .AddImports("System")
                   .AddReferences("System.Runtime, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a") // Doesn't work!?
                   .AddReferences("System.Runtime") // Doesn't work!?
                   .AddReferences("System.Private.Mscorlib") // Doesn't work!?
#if NET462
                .AddReferences(typeof(System.Security.Cryptography.Aes).GetTypeInfo().Assembly) // Doesn't work!?
                   .AddReferences("System.Security.Cryptography.Algorithms") // Doesn't work!?
#endif
                   .AddReferencesCore(typeof(MyGlobals).GetTypeInfo().Assembly)
                   .AddReferencesCore(typeof(object).GetTypeInfo().Assembly) // Doesn't work!?
                    .WithSourceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, ScriptsDirectory))
                    ;

                return options;
            }
        }
    }
    public static class ScriptOptionsExtensions
    {
        public static unsafe ScriptOptions AddReferencesCore(this ScriptOptions options, Assembly assembly)
        {
            // See http://www.strathweb.com/2016/03/roslyn-scripting-on-coreclr-net-cli-and-dnx-and-in-memory-assemblies/
#if NET462
                options.AddReferences(assembly);
#else
            byte* b;
            int length;
            if (assembly.TryGetRawMetadata(out b, out length))
            {
                var moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)b, length);
                var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
                var result = assemblyMetadata.GetReference();
                options.AddReferences(result);
            }
            else
            {
                throw new Exception("Failed to get raw metadata for assembly: " + assembly.FullName);
            }
#endif
            return options;
        }

    }
}
