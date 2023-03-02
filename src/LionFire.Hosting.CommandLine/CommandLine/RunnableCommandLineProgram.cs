using System;
using Microsoft.Extensions.Hosting;
using LionFire.Hosting;
using System.CommandLine;

namespace LionFire.Hosting;

//[Obsolete("TODO: Composition alternative")]
//public class RunnableCommandLineProgram<TConcrete> : CommandLineProgram<TConcrete>
//    where TConcrete : RunnableCommandLineProgram<TConcrete>
//{
//    public string RunCommandName { get; set; } = "run";

//    #region RunHostBuilder

//    public TConcrete RunHostBuilder(Action<IHostBuilder> action)
//    {
//        //if (HostApplicationBuilderActions != null) throw new NotImplementedException("Cannot mix IHostBuilder and HostApplicationBuilder yet");
//        (RunHostBuilderInitializer ??= new()).Add(action);
//        return (TConcrete)this;
//    }
//    Builder<IHostBuilder, HostBuilder> RunHostBuilderInitializer;

//    #endregion

//    [Obsolete]
//    protected override void OnBuildingCommandLine(RootCommand root)
//    {
//        root.AddCommand(new Command(RunCommandName) { Handler = CommandHandler.Create(() => (RunHostBuilderInitializer ??= new()).Create(ProgramHostBuilderInitializer.Create())._RunConsoleAsync()) });
//    }
//}

// OLD

//public static class CommandLineProgram
//{
//    public static CommandLineProgram RunCommand(this CommandLineProgram program, string command, Action<HostingBuilderBuilderContext, HostApplicationBuilder>? configure = null)
//    {
//        program.Command("run", (context, builder) => { 
//            configure?.Invoke(context, builder);
//            throw new NotImplementedException();
//        });
//        return program;
//    }
    
//    //public static CommandLineProgram Command(this CommandLineProgram program, string commandName, Action<CommandLineProgram, IHostBuilder> configure)
//    //{
//    //    root.AddCommand(new Command(commandName) { Handler = CommandHandler.Create(() => (program.HostBuilderInitializers RunHostBuilderInitializer ??= new()).Create(ProgramHostBuilderInitializer.Create())._RunConsoleAsync()) });
//    //    return program;
//    //}
//}