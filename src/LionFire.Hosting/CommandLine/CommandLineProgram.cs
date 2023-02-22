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
using System.ComponentModel.Design;
using LionFire.FlexObjects;
using System.CommandLine.Invocation;
using System.Linq;

namespace LionFire.Hosting.CommandLine;

// REVIEW: Is there a composition alternative to this?  Some sort of ProgramBuilder rather than inheritance.
// TODO Idea: programBuilder.HostBuilder("run", (hostBuilder, (IFlex) invocationContext) => ...)
// TODO Idea: programBuilder.HostBuilder("orleans db create", (hostBuilder, (IFlex) invocationContext) => ...)
// TODO Idea: programBuilder.HostBuilder("orleans db verify", (hostBuilder, (IFlex) invocationContext) => ... , noInherit: new string[] { "orleans", "orleans db" })
// TODO Idea: programBuilder.UseOrleans()

public class CommandLineProgram : ICommandLineProgram, IFlex
{
    #region Lifecycle

    /// <summary>
    /// 
    /// </summary>
    /// <param name="defaults">If true:
    ///  - UseConsoleLifetime()
    ///  </param>
    public CommandLineProgram(bool defaults = true)
    {
        Defaults = defaults;
    }

    #endregion

    #region State

    object? IFlex.FlexData { get; set; }

    public bool Defaults { get; }
    //Dictionary<object, object> Properties => this.Get<Dictionary<object, object>>();

    public RootCommand RootCommand { get; } = new();

    #endregion

    #region BuilderBuilders

    public IReadOnlyDictionary<string, IHostingBuilderBuilder> BuilderBuilders => builderBuilders;
    protected Dictionary<string, IHostingBuilderBuilder> builderBuilders { get; } = new();

    public TBuilderBuilder GetOrAdd<TBuilderBuilder>(string commandHierarchy)
        where TBuilderBuilder : IHostingBuilderBuilder, new()
    {
        if (BuilderBuilders.TryGetValue(commandHierarchy, out var hostingBuilderBuilder))
        {
            if (hostingBuilderBuilder is not TBuilderBuilder casted) throw new AlreadySetException("Cannot mix IHostingBuilderBuilder implementation types for same command");
            return casted;
        }
        return CreateBuilderAndCommand<TBuilderBuilder>(commandHierarchy);
    }

    public TBuilderBuilder Add<TBuilderBuilder>(string commandHierarchy)
      where TBuilderBuilder : IHostingBuilderBuilder, new()
    {
        if (BuilderBuilders.ContainsKey(commandHierarchy)) throw new AlreadySetException();

        return CreateBuilderAndCommand<TBuilderBuilder>(commandHierarchy);
    }

    private TBuilderBuilder CreateBuilderAndCommand<TBuilderBuilder>(string commandHierarchy) where TBuilderBuilder : IHostingBuilderBuilder, new()
    {
        var builderBuilder = new TBuilderBuilder();
        TryApplyDefaults(builderBuilder);

        builderBuilders.Add(commandHierarchy, builderBuilder);

        #region Create Command

        var command = RootCommand.GetOrCreateCommandFromHierarchy(commandHierarchy);
        command.SetHandler(Handler);
        builderBuilder.Command = command;

        #endregion

        return builderBuilder;
    }


    public virtual void TryApplyDefaults(IHostingBuilderBuilder builderBuilder)
    {
        if (!Defaults) return;
        builderBuilder.UseConsoleLifetime();
    }

    //#region IHostBuilder

    //public TConcrete HostBuilder(Action<IHostBuilder> action) => HostBuilder("", action);
    //public TConcrete HostBuilder(string command, Action<IHostBuilder> action) => HostBuilder(command, (_, builder) => action(builder));
    //public TConcrete HostBuilder(string command, Action<HostingBuilderBuilderContext, IHostBuilder> action)
    //{
    //    IHostingBuilderBuilder? builderBuilder;
    //    BuilderBuilderBase<IHostBuilder>? hostBuilderBuilder;

    //    if (command.Contains("  ")) throw new ArgumentException("command must have single spaces between all subcommands");

    //    if (BuilderBuilders.TryGetValue(command, out builderBuilder))
    //    {
    //        if (builderBuilder is HostApplicationBuilderBuilder) { throw new Exception("Cannot mix with HostApplicationBuilder"); }
    //        hostBuilderBuilder = builderBuilder as BuilderBuilderBase<IHostBuilder>;
    //        if (hostBuilderBuilder == null) throw new Exception("Unknown IHostingBuilderBuilder type");
    //    }
    //    else
    //    {
    //        hostBuilderBuilder = new HostBuilderBuilder();
    //        BuilderBuilders.Add(command, hostBuilderBuilder);
    //    }

    //    hostBuilderBuilder.Initializers.Add(action);

    //    return (TConcrete)this;
    //}
    //#endregion

    //#region HostApplicationBuilder

    //public TConcrete HostApplicationBuilder(Action<HostApplicationBuilder> action) => programHostBuilder("", action);
    //public TConcrete HostApplicationBuilder(string command, Action<HostApplicationBuilder> action) => HostBuilder(command, (_, builder) => action(builder));
    //public TConcrete HostApplicationBuilder(string command, Action<HostingBuilderBuilderContext, HostApplicationBuilder> action)
    //{
    //    IHostingBuilderBuilder? builderBuilder;
    //    BuilderBuilderBase<HostApplicationBuilder>? hostApplicationBuilderBuilder;

    //    if (command.Contains("  ")) throw new ArgumentException("command must have single spaces between all subcommands");

    //    if (BuilderBuilders.TryGetValue(command, out builderBuilder))
    //    {
    //        if (builderBuilder is HostApplicationBuilderBuilder) { throw new Exception("Cannot mix with HostApplicationBuilder"); }
    //        hostApplicationBuilderBuilder = builderBuilder as BuilderBuilderBase<HostApplicationBuilder>;
    //        if (hostApplicationBuilderBuilder == null) throw new Exception("Unknown IHostingBuilderBuilder type");
    //    }
    //    else
    //    {
    //        hostApplicationBuilderBuilder = new HostBuilderBuilder();
    //        BuilderBuilders.Add(command, hostApplicationBuilderBuilder);
    //    }

    //    hostApplicationBuilderBuilder.Initializers.Add(action);

    //    return (TConcrete)this;
    //}
    //#endregion

    #endregion

    //if (Defaults) // TODO TRIAGE 
    //    programHostBuilderInitializer.Add(hb => hb.LionFire());

    #region FUTURE

    //public bool UseGlobalHostBuilder { get; set; } = false;  // FUTURE maybe
    // Func<IHostBuilder> GlobalHostBuilderInitializer

    #endregion

    public Task<int> RunAsync(string[] args)
    {
        return args.RunWithExitCodeAsync(args2 =>
                 new CommandLineBuilder(RootCommand)
                    //.If(GlobalHost, casted => casted.UseHost(args3 => GlobalHostBuilderInitializer.Create()) // FUTURE Maybe
                    .UseDefaults()
                    .Build()
                    .InvokeAsync(args2)
            );
    }

    public Task Handler(InvocationContext invocationContext)
    {
        return this.GetBuilderBuilderHierarchy(invocationContext).Reverse().FirstOrDefault()?.RunAsync(this, invocationContext) ?? Task.CompletedTask;
        //foreach (var bb in )
        //{
        //    bb.RunAsync(this, invocationContext);
        //    break;
        //}
    }

}
