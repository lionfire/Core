using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

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
                    .WithSourceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, ScriptsDirectory))
                    ;
                return options;
            }
        }
    }
}
