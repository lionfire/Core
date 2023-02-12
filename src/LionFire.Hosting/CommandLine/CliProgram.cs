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

namespace LionFire.Hosting;

//static void RunSilo(RunSiloOptions runSiloOptions, IHost host)
//{
//    IHostApplicationLifetime lifetime = host.Services.GetService<IHostApplicationLifetime>();
//    var siloOptions = host.Services.GetService<IOptions<Orleans.Configuration.SiloOptions>>();

//    Console.WriteLine("Silo running: " + siloOptions?.Value?.SiloName);
//    lifetime.ApplicationStopping.WaitHandle.WaitOneAsync().Wait();
//    Console.WriteLine("Silo done.");
//}

public class Initializer<T, TImplementation>
    where TImplementation : T, new()
{
    public T Create()
    {
        var result = new TImplementation();
        foreach (var initializer in Initializers)
        {
            initializer(result);
        }
        return result;
    }
    public void Add(Action<T> a)
    {
        Initializers ??= new();
        Initializers.Add(a);
    }

    public List<Action<T>> Initializers { get; set; }
}

public class CliProgram<TConcrete>
    where TConcrete : CliProgram<TConcrete>

{
    public bool Defaults { get; }

    public CliProgram(bool defaults = true)
    {
        Defaults = defaults;
    }

    //public Func<HostApplicationBuilder> ProgramHostApplicationBuilder { get; set; } // TODO - add this, or replace ProgramHostBuilder?
    public Action<RootCommand>? CommandLine { get; set; }

    #region ProgramHostBuilder

    public TConcrete ProgramHostBuilder(Action<IHostBuilder> action)
    {
        //if (HostApplicationBuilderActions != null) throw new NotImplementedException("Cannot mix IHostBuilder and HostApplicationBuilder yet");
        ProgramHostBuilderInitializer.Add(action);
        return (TConcrete)this;
    }

    Initializer<IHostBuilder, HostBuilder> ProgramHostBuilderInitializer
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
    Initializer<IHostBuilder, HostBuilder> programHostBuilderInitializer;


    #region HostApplicationBuilder

    private List<Action<HostApplicationBuilder>> HostApplicationBuilderActions;
#if UNUSED // Needs Microsoft to support it in System.CommandLine.Hosting?  Or is there an adapter I made?
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
                    .UseHost(args3 => ProgramHostBuilderInitializer.Create())
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
