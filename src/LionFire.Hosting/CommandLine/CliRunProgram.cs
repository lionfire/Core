using System;
using Microsoft.Extensions.Hosting;
using LionFire.Hosting;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace LionFire.Hosting;

public class CliRunProgram<TConcrete> : CliProgram<TConcrete>
    where TConcrete : CliRunProgram<TConcrete>
{
    #region RunHostBuilder

    public TConcrete RunHostBuilder(Action<IHostBuilder> action)
    {
        //if (HostApplicationBuilderActions != null) throw new NotImplementedException("Cannot mix IHostBuilder and HostApplicationBuilder yet");
        (RunHostBuilderInitializer ??= new()).Add(action);
        return (TConcrete)this;
    }
    Initializer<IHostBuilder, HostBuilder> RunHostBuilderInitializer;

    #endregion

    protected override void OnBuildingCommandLine(RootCommand root)
    {
        root.AddCommand(new Command("run") { Handler = CommandHandler.Create(() => (RunHostBuilderInitializer ??= new()).Create().RunConsoleAsync()) });

    }
}
