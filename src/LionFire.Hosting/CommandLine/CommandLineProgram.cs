#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using LionFire.Threading;
using LionFire.Hosting.ExitCode;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;
using System.ComponentModel.Design;

namespace LionFire.Hosting;

public class CommandLineProgram<TConcrete> : ICommandLineProgram
    where TConcrete : CommandLineProgram<TConcrete>
{
    public bool Defaults { get; }

    public CommandLineProgram(bool defaults = true)
    {
        Defaults = defaults;
    }

    public Action<RootCommand>? CommandLine { get; set; }

    #region ProgramHostBuilder

    public TConcrete ProgramHostBuilder(Action<IHostBuilder> action)
    {
        //if (HostApplicationBuilderActions != null) throw new NotImplementedException("Cannot mix IHostBuilder and HostApplicationBuilder yet");
        ProgramHostBuilderInitializer.Add(action);
        return (TConcrete)this;
    }

    IInitializer<IHostBuilder> ICommandLineProgram.ProgramHostBuilderInitializer => this.ProgramHostBuilderInitializer;
    protected Builder<IHostBuilder, HostBuilder> ProgramHostBuilderInitializer
    {
        get
        {
            if (programHostBuilderInitializer == null)
            {
                programHostBuilderInitializer = new();
                if (Defaults)
                {
                    programHostBuilderInitializer.Add(hb => hb.LionFire());
                    //if (AppInfo.IsFirstPartyLionFire) { 
                    //    programHostBuilderInitializer.Add(hb => hb.LionFireAppInfo());
                    //}
                }
            }
            return programHostBuilderInitializer;
        }
    }
    Builder<IHostBuilder, HostBuilder>? programHostBuilderInitializer;


    #region HostApplicationBuilder

#if UNUSED // Needs Microsoft to support it in System.CommandLine.Hosting?  Or is there an adapter I made?
    private List<Action<HostApplicationBuilder>>? HostApplicationBuilderActions;
    public CliProgram ProgramHostApplicationBuilder(Action<IHostBuilder> action)
    {
        if (HostBuilderActions != null) throw new NotImplementedException("Cannot mix IHostBuilder and HostApplicationBuilder yet");
        HostApplicationBuilderActions ??= new();
        HostApplicationBuilderActions.Add(action);
        return this;
    }
    protected HostApplicationBuilder CreateProgramHostApplicationBuilder()
    {
        var hab = new HostApplicationBuilder();
        foreach (var a in HostApplicationBuilderActions)
        {
            a(hab);
        }
        return hab;
    }
#endif

    #endregion

    #endregion


    public Task<int> Run(string[] args)
    {
        return args.RunWithExitCodeAsync(args2 =>
                 BuildCommandLine()
                    //.UseHost(args3 => ProgramHostBuilderInitializer.Create())
                    .UseDefaults()
                    .Build()
                    .InvokeAsync(args2)
            );
    }

    protected virtual CommandLineBuilder BuildCommandLine()
    {
        var root = new RootCommand();
        //root.Handler = CommandHandler.Create<RunSiloOptions, IHost>(RunSilo);

        OnBuildingCommandLine(root);

        CommandLine?.Invoke(root);


        return new CommandLineBuilder(root);
    }

    protected virtual void OnBuildingCommandLine(RootCommand root) { }
}
