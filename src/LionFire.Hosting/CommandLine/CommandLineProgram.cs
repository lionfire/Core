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

// REVIEW: API ideas
//   programBuilder.HostBuilder("run", (hostBuilder, (IFlex) invocationContext) => ...)
//   programBuilder.HostBuilder("orleans db create", (hostBuilder, (IFlex) invocationContext) => ...)
//   programBuilder.HostBuilder("orleans db verify", (hostBuilder, (IFlex) invocationContext) => ... , noInherit: new string[] { "orleans", "orleans db" })

public class HostBuilderProgram : CommandLineProgram<IHostBuilder, HostBuilder_.HostBuilderBuilder> { }
public class HostApplicationBuilderProgram : CommandLineProgram<HostApplicationBuilder, HostApplicationBuilder_.HostApplicationBuilderBuilder> { }


public class CommandLineProgram<TBuilder, TBuilderBuilder> : CommandLineProgram
    where TBuilderBuilder : IHostingBuilderBuilder<TBuilder>
{
    // ENH: replace parameters with Action<BuilderBuilderBuilder> (:-D) with fluent API 
    public CommandLineProgram<TBuilder, TBuilderBuilder> RootCommand(Action<TBuilder> builder, Action<TBuilderBuilder>? builderBuilder = null, Action<Command>? command = null) => Command("", builder, builderBuilder: builderBuilder, command: command);
    public CommandLineProgram<TBuilder, TBuilderBuilder> RootCommand(Action<HostingBuilderBuilderContext, TBuilder> builder, Action<TBuilderBuilder>? builderBuilder = null, Action<Command>? command = null) => Command("", builder, builderBuilder: builderBuilder, command: command);
    public CommandLineProgram<TBuilder, TBuilderBuilder> Command(string commandHierarchy, Action<TBuilder> builder, Action<TBuilderBuilder>? builderBuilder = null, Action<Command>? command = null) => Command(commandHierarchy, (_, b) => builder(b), builderBuilder: builderBuilder, command: command);
    public CommandLineProgram<TBuilder, TBuilderBuilder> Command(string commandHierarchy, Action<HostingBuilderBuilderContext, TBuilder>? builder = null, Action<TBuilderBuilder>? builderBuilder = null, Action<Command>? command = null)
    {
        CommandLineProgramValidation.ValidateCommand(commandHierarchy);

        var hostBuilderBuilder = GetOrAdd<TBuilderBuilder>(commandHierarchy);
        builderBuilder?.Invoke(hostBuilderBuilder);
        command?.Invoke(hostBuilderBuilder.Command!);
        if (builder != null) hostBuilderBuilder.AddInitializer(builder);

        return this;
    }
}

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
        where TBuilderBuilder : IHostingBuilderBuilder
    {
        if (BuilderBuilders.TryGetValue(commandHierarchy, out var hostingBuilderBuilder))
        {
            if (hostingBuilderBuilder is not TBuilderBuilder casted) throw new AlreadySetException("Cannot mix IHostingBuilderBuilder implementation types for same command");
            return casted;
        }
        return CreateBuilderAndCommand<TBuilderBuilder>(commandHierarchy);
    }

    public TBuilderBuilder Add<TBuilderBuilder>(string commandHierarchy)
      where TBuilderBuilder : IHostingBuilderBuilder
    {
        if (BuilderBuilders.ContainsKey(commandHierarchy)) throw new AlreadySetException();

        return CreateBuilderAndCommand<TBuilderBuilder>(commandHierarchy);
    }

    private TBuilderBuilder CreateBuilderAndCommand<TBuilderBuilder>(string commandHierarchy) where TBuilderBuilder : IHostingBuilderBuilder
    {
        #region Create Command

        var command = RootCommand.GetOrCreateCommandFromHierarchy(commandHierarchy);

        #endregion

        var builderBuilder = (TBuilderBuilder)Activator.CreateInstance(typeof(TBuilderBuilder))!;
        builderBuilder.Program = this;
        builderBuilder.Command = command;
        builderBuilder.CommandHierarchy = commandHierarchy;
        TryApplyDefaults(builderBuilder);

        builderBuilders.Add(commandHierarchy, builderBuilder);

        return builderBuilder;
    }

    public virtual void TryApplyDefaults(IHostingBuilderBuilder builderBuilder)
    {
        if (!Defaults) return;
        //builderBuilder.UseConsoleLifetime();
    }

    #endregion

    #region FUTURE

    //public bool UseGlobalHostBuilder { get; set; } = false;  // FUTURE maybe
    // Func<IHostBuilder> GlobalHostBuilderInitializer

    #endregion

    public Task<int> RunAsync(string[] args)
        => new CommandLineBuilder(RootCommand)
                    //.If(GlobalHost, casted => casted.UseHost(args3 => GlobalHostBuilderInitializer.Create()) // FUTURE Maybe
                    .UseDefaults()
                    .Build()
                    .InvokeAsync(args)
            ;

    public Task<int> Handler(InvocationContext invocationContext) => this.GetBuilderBuilderHierarchy(invocationContext).FirstOrDefault()?.RunAsync(this, invocationContext) ?? Task.FromResult(0);

}
