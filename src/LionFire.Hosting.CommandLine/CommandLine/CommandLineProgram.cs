﻿#nullable enable
//#define BuilderBuilderBuilder  // replaces parameters with Action<BuilderBuilderBuilder> with fluent API, but it may be more natural to deal with the multiple parameters

using System;
using LionFire.Threading;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using LionFire.FlexObjects;
using System.CommandLine.Invocation;
using LionFire.ExtensionMethods;
using System.CommandLine.Binding;

namespace LionFire.Hosting.CommandLine;

// FUTURE ENH - flesh out inheritance API ideas:
// - Inherit (ancestors)
// - Inherited (by children)

public static class LionFireOptionBinderX
{
    public static void AddParsedValues(this Dictionary<string, object?> o, IEnumerable<Option> Options, BindingContext bindingContext)
    {
        foreach (var option in Options.Where(v => !o.ContainsKey(v.Name)))
        {
            o.Add(option.Name, bindingContext.ParseResult.GetValueForOption(option));
        }
    }
}

//public class LionFireOptionBinder : BinderBase<LionFireCommandLineOptions>
//{
//    public LionFireOptionBinder(List<Option> options)
//    {
//        Options = options;
//    }

//    public List<Option> Options { get; }

//    protected override LionFireCommandLineOptions GetBoundValue(BindingContext bindingContext)
//    {

//        var result = new LionFireCommandLineOptions();

//        foreach (var option in Options)
//        {
//            result.Options.Add(option.Name, bindingContext.ParseResult.GetValueForOption(option));
//        }

//        return result;
//    }
//}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TBuilder">Default (and currently only) builder class (e.g. HostApplicationBuilder)</typeparam>
/// <typeparam name="TBuilderBuilder">BuilderBuilder class, for configuring the TBuilder</typeparam>
public class CommandLineProgram<TBuilder, TBuilderBuilder> : CommandLineProgram
    where TBuilderBuilder : IHostingBuilderBuilder<TBuilder>, new()
{
#if false // ENH Maybe - there is already inheritance
    public CommandLineProgram<TBuilder, TBuilderBuilder> Common(Action<HostingBuilderBuilderContext, TBuilder> initializer)
    {
        GetCommonBuilderBuilder<TBuilderBuilder, TBuilder>().AddInitializer(initializer);
        return this;
    }
    public CommandLineProgram<TBuilder, TBuilderBuilder> Common(Action<TBuilder> initializer)
    {
        GetCommonBuilderBuilder<TBuilderBuilder, TBuilder>().AddInitializer((_, b) => initializer(b));
        return this;
    }
#endif


    // REVIEW: the API style. command is accessible via builderBuilder, so it could be removed as a parameter here.  Or could/should builderBuilder be removed?  

    public new CommandLineProgram<TBuilder, TBuilderBuilder> RootCommand(Action<TBuilder> builder, Action<TBuilderBuilder>? builderBuilder = null, Action<Command>? command = null) => Command("", builder, builderBuilder: builderBuilder, command: command);
    public new CommandLineProgram<TBuilder, TBuilderBuilder> RootCommand(Action<HostingBuilderBuilderContext, TBuilder> builder, Action<TBuilderBuilder>? builderBuilder = null, Action<Command>? command = null) => Command("", builder, builderBuilder: builderBuilder, command: command);


    public CommandLineProgram<TBuilder, TBuilderBuilder> Command(string commandHierarchy, Action<TBuilder> builder, Action<TBuilderBuilder>? builderBuilder = null, Action<Command>? command = null, Type? optionsType = null) => Command(commandHierarchy, (_, b) => builder(b), builderBuilder: builderBuilder, command: command, optionsType: optionsType);

    public CommandLineProgram<TBuilder, TBuilderBuilder> Command(string commandHierarchy, Action<HostingBuilderBuilderContext, TBuilder>? builder = null, Action<TBuilderBuilder>? builderBuilder = null, Action<Command>? command = null, Type? optionsType = null)
    {
        CommandLineProgramValidation.ValidateCommand(commandHierarchy);

        var hostBuilderBuilder = GetOrAdd<TBuilderBuilder>(commandHierarchy);
        hostBuilderBuilder.OptionsType = optionsType;
        builderBuilder?.Invoke(hostBuilderBuilder);
        command?.Invoke(hostBuilderBuilder.Command!);
        if (builder != null) hostBuilderBuilder.AddInitializer(builder);

        return this;
    }

    #region Command: OptionsType as generic parameter

    public CommandLineProgram<TBuilder, TBuilderBuilder> Command<TOptions>(string commandHierarchy, Action<TBuilder> builder, Action<TBuilderBuilder>? builderBuilder = null, Action<Command>? command = null) => Command<TOptions>(commandHierarchy, (_, b) => builder(b), builderBuilder: builderBuilder, command: command);

    public CommandLineProgram<TBuilder, TBuilderBuilder> Command<TOptions>(string commandHierarchy, Action<HostingBuilderBuilderContext, TBuilder>? builder = null, Action<TBuilderBuilder>? builderBuilder = null, Action<Command>? command = null)
        => Command(commandHierarchy, builder, builderBuilder, command, typeof(TOptions));

    #endregion

#if BuilderBuilderBuilder
    public new CommandLineProgram<TBuilder, TBuilderBuilder> RootCommandBuilder(Action<HostingBuilderBuilderBuilder<TBuilder, TBuilderBuilder>>? builderBuilder = null) => CommandBuilder("", builderBuilder);
    public CommandLineProgram<TBuilder, TBuilderBuilder> CommandBuilder(string commandHierarchy, Action<HostingBuilderBuilderBuilder<TBuilder, TBuilderBuilder>>? builderBuilder = null)
    {
        CommandLineProgramValidation.ValidateCommand(commandHierarchy);

        var hostBuilderBuilder = GetOrAdd<TBuilderBuilder>(commandHierarchy);

        if (builderBuilder != null)
        {
            builderBuilder(new HostingBuilderBuilderBuilder<TBuilder, TBuilderBuilder>(hostBuilderBuilder));
        }

        return this;
    }
#endif
}

#if BuilderBuilderBuilder

public class HostingBuilderBuilderBuilder<TBuilder, TBuilderBuilder>
    where TBuilderBuilder : IHostingBuilderBuilder<TBuilder>
{
    public TBuilderBuilder HostBuilderBuilder { get; init; }

    public HostingBuilderBuilderBuilder(TBuilderBuilder hostBuilderBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilderBuilder);
        HostBuilderBuilder = hostBuilderBuilder;
    }
}

public static class HostingBuilderBuilderBuilderX
{
    public static HostingBuilderBuilderBuilder<TBuilder, TBuilderBuilder> Builder<TBuilder, TBuilderBuilder>(this HostingBuilderBuilderBuilder<TBuilder, TBuilderBuilder> bbb, Action<HostingBuilderBuilderContext, TBuilder> builder)
      where TBuilderBuilder : IHostingBuilderBuilder<TBuilder>
    {
        bbb.HostBuilderBuilder.AddInitializer(builder);
        return bbb;
    }

    public static HostingBuilderBuilderBuilder<TBuilder, TBuilderBuilder> Command<TBuilder, TBuilderBuilder>(this HostingBuilderBuilderBuilder<TBuilder, TBuilderBuilder> bbb, Action<Command> command)
        where TBuilderBuilder : IHostingBuilderBuilder<TBuilder>
    {
        ArgumentNullException.ThrowIfNull(bbb.HostBuilderBuilder.Command);
        command(bbb.HostBuilderBuilder.Command);
        return bbb;
    }
}
#endif

public class CommandLineProgram : IProgram, IFlex
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

    public IHostingBuilderBuilder<TBuilder> GetCommonBuilderBuilder<TBuilderBuilder, TBuilder>()
        where TBuilderBuilder : IHostingBuilderBuilder<TBuilder>, new()
        //where TBuilder : IHostingBuilderBuilder<TBuilder>, new()
    {
        if (SharedBuilderBuilders.TryGetValue("", out var hostingBuilderBuilder))
        {
            if (hostingBuilderBuilder is not TBuilderBuilder casted) throw new AlreadySetException("Cannot mix IHostingBuilderBuilder implementation types for same command");
            return casted;
        }

        var result = new TBuilderBuilder();
        sharedBuilderBuilders.Add("", result);
        return result;
    }

    public IReadOnlyDictionary<string, IHostingBuilderBuilder> SharedBuilderBuilders => builderBuilders;
    protected Dictionary<string, IHostingBuilderBuilder> sharedBuilderBuilders { get; } = new();

    public IReadOnlyDictionary<string, IHostingBuilderBuilder> BuilderBuilders => builderBuilders;
    protected Dictionary<string, IHostingBuilderBuilder> builderBuilders { get; } = new();

    protected TBuilderBuilder GetOrAdd<TBuilderBuilder>(string commandHierarchy)
        where TBuilderBuilder : IHostingBuilderBuilder
    {
        if (BuilderBuilders.TryGetValue(commandHierarchy, out var hostingBuilderBuilder))
        {
            if (hostingBuilderBuilder is not TBuilderBuilder casted) throw new AlreadySetException("Cannot mix IHostingBuilderBuilder implementation types for same command");
            return casted;
        }
        return CreateBuilderAndCommand<TBuilderBuilder>(commandHierarchy);
    }

    protected TBuilderBuilder Add<TBuilderBuilder>(string commandHierarchy)
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
        // ENH: Replace with common ctor arguments?
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
                    .InvokeAsync(args.Length == 0 ? DefaultArgsList : args)
            ;

    public Task<int> Handler(InvocationContext invocationContext)
        => this.GetBuilderBuilderHierarchy(invocationContext).FirstOrDefault()?.RunAsync(this, invocationContext) ?? Task.FromResult(0);

    public string[] DefaultArgsList { get; set; } = Array.Empty<string>();

}

