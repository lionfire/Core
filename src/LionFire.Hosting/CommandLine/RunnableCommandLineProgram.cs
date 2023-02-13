using System;
using Microsoft.Extensions.Hosting;
using LionFire.Hosting;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace LionFire.Hosting;

public class RunnableCommandLineProgram<TConcrete> : CommandLineProgram<TConcrete>
    where TConcrete : RunnableCommandLineProgram<TConcrete>
{
    public string RunCommandName { get; set; } = "run";

    #region RunHostBuilder

    public TConcrete RunHostBuilder(Action<IHostBuilder> action)
    {
        //if (HostApplicationBuilderActions != null) throw new NotImplementedException("Cannot mix IHostBuilder and HostApplicationBuilder yet");
        (RunHostBuilderInitializer ??= new()).Add(action);
        return (TConcrete)this;
    }
    Builder<IHostBuilder, HostBuilder> RunHostBuilderInitializer;

    #endregion

    protected override void OnBuildingCommandLine(RootCommand root)
    {
        root.AddCommand(new Command(RunCommandName) { Handler = CommandHandler.Create(() => (RunHostBuilderInitializer ??= new()).Create(ProgramHostBuilderInitializer.Create()).RunConsoleAsync()) });
    }
}
