#define GLOBALS
using LionFire.Execution;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.Reactive.Subjects;

namespace LionFire.Execution.Roslyn.Scripting
{
    public class MyGlobals
    {
        public string A = "123";
        public int B = 456;
    }

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
            globals = new Dictionary<string, object>();
            globals.Add("TestGlobal", "Testglobalval");
            globals.Add("TestGlobal2", 123);
            loader.RegisterDependency(typeof(object).GetTypeInfo().Assembly);
        }

        #endregion

        #region Derived Properties

        public string ScriptCode {
            get {
                return ExecutionContext.Config.SourceContent as string;
            }
        }

        #endregion

        Dictionary<string, object> globals = new Dictionary<string, object>();

        #region State

        #region ExecutionState

        public ExecutionState ExecutionState {
            get {
                return bExecutionState.Value;
            }
            set {
                bExecutionState.OnNext(value);
            }
        }

        public IObservable<ExecutionState> ExecutionStates {
            get {
                return bExecutionState;
            }
        }
        private BehaviorSubject<ExecutionState> bExecutionState = new BehaviorSubject<ExecutionState>(ExecutionState.Unspecified);

        #endregion


        Script<object> script;

        Task runTask = null;
        CancellationTokenSource cts;

        #endregion

        #region (Public) IExecutable Implementation

        Microsoft.CodeAnalysis.Scripting.Hosting.InteractiveAssemblyLoader loader = new Microsoft.CodeAnalysis.Scripting.Hosting.InteractiveAssemblyLoader();
        public Task<bool> Initialize()
        {
            //CSharpScriptEngine.Execute(


            script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(ScriptCode
            , ScriptContext.Options
#if GLOBALS
            , typeof(MyGlobals)
#endif
            , loader
            );

            // TODO REFACTOR REVIEW - controllers should opt in to adding these flags and not be forced to turn them off
            ExecutionContext.InitializationState &= ~(ExecutionContextInitializationState.MissingCode | ExecutionContextInitializationState.MissingConstructor);
            // TODO: Review whether constructor can be determined here

            return Task.FromResult(script != null);
        }


        public Task Start()
        {
            cts = new System.Threading.CancellationTokenSource();
#if GLOBALS
            var roslynRunTask = script.RunAsync(new MyGlobals(), cts.Token);
#else
            var roslynRunTask = script.RunAsync(cancellationToken:cts.Token);
#endif

            runTask = Task.Run(() =>
            {
                roslynRunTask.Wait();
                var result = roslynRunTask.Result;
                if (result.Exception != null)
                {
                    Console.WriteLine("Roslyn script exception: " + Environment.NewLine + result.Exception.ToString());
                }
                if (result.ReturnValue != null)
                {
                    Console.WriteLine("Roslyn ReturnValue: " + result.ReturnValue);
                }
                foreach (var variable in result.Variables)
                {
                    Console.WriteLine($" - {variable.Name} = {variable.Value}");
                }
            });

            return Task.CompletedTask;
        }

        public Task Stop(StopMode mode = StopMode.GracefulShutdown, StopOptions options = StopOptions.StopChildren)
        {
            if (runTask == null || runTask.IsCompleted)
            {
                return Task.CompletedTask;
            }
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
            }
            if (mode == StopMode.GracefulShutdown)
            {
                return Task.Run(() => { runTask.Wait(); });
            }
            else
            {
                // REVIEW
                return Task.CompletedTask;
            }
        }

        #endregion

    }
}






//public ScriptOptions Options {
//    get {

//        var options = ScriptOptions.Default
//            .AddImports("System")
//            .AddReferences("System.Runtime, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a") // Doesn't work!?
//            .AddReferences("System.Runtime") // Doesn't work!?
//            .AddReferences("System.Private.Mscorlib") // Doesn't work!?
//   #if NET462 || NET461
//                .AddReferences(typeof(System.Security.Cryptography.Aes).GetTypeInfo().Assembly) // Doesn't work!?
//                    .AddReferences("System.Security.Cryptography.Algorithms") // Doesn't work!?
//    #endif
//                    .AddReferencesCore(typeof(MyGlobals).GetTypeInfo().Assembly)
//            .AddReferencesCore(typeof(object).GetTypeInfo().Assembly) // Doesn't work!?
//            .WithSourceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, ScriptsDirectory))
//            ;
//        return options;
//    }
//}

//public static unsafe ScriptOptions AddReferencesCore(this ScriptOptions options, Assembly assembly)
//{
//    // See http://www.strathweb.com/2016/03/roslyn-scripting-on-coreclr-net-cli-and-dnx-and-in-memory-assemblies/
//#if NET462 || NET461
//            options.AddReferences(assembly);
//#else
//    byte* b;
//    int length;
//    if (assembly.TryGetRawMetadata(out b, out length))
//    {
//        var moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)b, length);
//        var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
//        var result = assemblyMetadata.GetReference();
//        options.AddReferences(result);
//    }
//    else
//    {
//        throw new Exception("Failed to get raw metadata for assembly: " + assembly.FullName);
//    }
//#endif
//    return options;
//}

//Microsoft.CodeAnalysis.Scripting.Hosting.InteractiveAssemblyLoader loader = new Microsoft.CodeAnalysis.Scripting.Hosting.InteractiveAssemblyLoader();

//public Task<bool> Initialize()
//{
//    //CSharpScriptEngine.Execute(
//    script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(myCSharpCodeString
//    , Options
//#if GLOBALS
//        , typeof(MyGlobals)
//#endif
//        , loader
//    );

//    return Task.FromResult(script != null);
//}


//public Task Start()
//{
//    cts = new System.Threading.CancellationTokenSource();
//#if GLOBALS
//    var roslynRunTask = script.RunAsync(new MyGlobals(), cts.Token); // Object type not found, please add reference to System.Runtime, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
//#else
//        var roslynRunTask = script.RunAsync(cancellationToken:cts.Token);
//#endif
//    // ...
//}

//public class MyGlobals
//{
//    public string A = "123";
//    public int B = 456;
//}
