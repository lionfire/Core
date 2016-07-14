using LionFire.Execution;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Execution.Roslyn.Scripting
{
    public class RoslynScriptExecutionController : IExecutionController
    {
        #region Relationships

        public ExecutionContext ExecutionContext { get; set; }

        public RoslynScriptContext ScriptContext { get; private set; }

        #endregion

        #region Construction

        public RoslynScriptExecutionController(ExecutionContext ec)
        {
            this.ExecutionContext = ec;
            ScriptContext = new RoslynScriptContext();
        }

        #endregion

        public string Code {
            get {
                return ExecutionContext.ExecutionObject as string;
            }
        }

        Script<object> script;

        #region (Public) IExecutable Implementation

        public async Task Start()
        {
            var result = await script.RunAsync();
            return;
        }

        public Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Initialize()
        {
            script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(Code
            //CSharpScriptEngine.Execute(
            , ScriptContext.Options);

            return script != null;
        }

        #endregion

    }
}
