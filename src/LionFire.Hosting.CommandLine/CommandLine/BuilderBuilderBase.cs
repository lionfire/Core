#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.Diagnostics;

namespace LionFire.Hosting.CommandLine;

public class LionFireCommandLineOptions
{
    public Dictionary<string, object?> Options { get; set; }
    public InvocationContext InvocationContext { get; internal set; }

    public T As<T>() where T : new()
    {
        var result = new T();

        if (Options != null)
        {
            Type type = typeof(T);
            foreach (var keyValue in Options)
            {
                type.GetProperty(keyValue.Key)?.SetValue(result, keyValue.Value, null);
            }
        }

        return result;
    }
}


// Supported TBuilder types:
//  - IHostBuilder
//  - HostApplicationBuilder
public abstract class BuilderBuilderBase<TBuilder> : IHostingBuilderBuilder<TBuilder>
{

    #region Relationships

    public Command? Command { get; set; }
    public IProgram? Program { get; set; }

    public Type? OptionsType { get; set; }

    #endregion

    #region Identity

    public Type BuilderType => typeof(TBuilder);

    /// <summary>
    /// Commands separated by single spaces
    /// </summary>
    public string? CommandHierarchy { get; set; }

    #endregion

    #region Parameters

    /// <summary>
    /// Inherit HostApplicationBuilder from parent commands.
    /// Default: true
    /// </summary>
    public bool Inherit { get; set; } = true;
    public IReadOnlyList<Action<HostingBuilderBuilderContext, TBuilder>> Initializers => initializers;


    protected List<Action<HostingBuilderBuilderContext, TBuilder>> initializers;
    public void AddInitializer(Action<HostingBuilderBuilderContext, TBuilder> initializer)
    {
        if (initializers == null)
        {
            initializers = new();
            Command.SetHandler(Program.Handler);
        }
        initializers.Add(initializer);
    }

    public void Initialize(HostingBuilderBuilderContext context, TBuilder builder)
    {
        foreach (var init in Initializers)
        {
            init(context, builder);
        }
    }

    #endregion

    #region Methods

    public void InitializeHierarchy(IProgram program, InvocationContext invocationContext, HostingBuilderBuilderContext context, TBuilder builder)
    {
        foreach (var bb in program.GetBuilderBuilderHierarchy(invocationContext).Reverse())
        {
            if (bb is not IHostingBuilderBuilder<TBuilder> bbCasted)
            {
                throw new Exception($"Mismatch in hierarchy of host builder types: expecting IHostingBuilderBuilder<{typeof(TBuilder)}> but got {bb.GetType().Name}.  Either align types, or set IHostingBuilderBuilder.Inherit to false");
            }
            Debug.WriteLine($"Initializing for {bb.Command?.GetType().Name}: " + bb.Command?.Name);
            context.InitializingForCommandName = bb.Command?.Name;

            context.Options.AddParsedValues(invocationContext.BindingContext.ParseResult.CommandResult.Command.Options, invocationContext.BindingContext); // TODO: parse parent command options as well

            var properties = ObjectToHostBuilderX.GetProperties(builder);
            //(builder as IHostBuilder)?.Properties ?? (builder as HostApplicationBuilder)?.Properties(); // OLD

            properties?.TryAdd("Options", context.Options); // REVIEW - if already added, need to merge?

            var configureServices = ObjectToHostBuilderX.GetConfigureServices(builder);

            configureServices(s => s
                .AddSingleton(new LionFireCommandLineOptions
                {
                    Options = context.Options,
                    InvocationContext = invocationContext
                }));

            //var hostBuilder = builder as IHostBuilder ?? (builder as HostApplicationBuilder)?.AsHostBuilder();

            if (OptionsType != null)
            {
                var binder = new ModelBinder(OptionsType);
                var optionsObject = binder.CreateInstance(invocationContext.BindingContext);
                if (optionsObject != null)
                {
                    configureServices(s => s.AddSingleton(OptionsType, optionsObject));
                }
            }

            try
            {
                bbCasted.Initialize(context, builder);
            }
            finally
            {
                context.InitializingForCommandName = null;
            }
        }
    }

    #region Pass-thru

    public abstract IHostingBuilderBuilder ConfigureServices(Action<IServiceCollection> services);

    protected TBuilder? Builder;

    public IHost Build(IProgram program, InvocationContext invocationContext)
    {
        if (Builder != null) { throw new AlreadyException(); }
        try
        {
            Builder = CreateBuilder();

            var context = new HostingBuilderBuilderContext
            {
                HostingBuilderBuilder = this,
                Program = program,
                InvocationContext = invocationContext,
            };

            InitializeHierarchy(program, invocationContext, context, Builder);

            return Build(Builder);
        }
        finally
        {
            Builder = default;
        }
    }

    public abstract IHost Build(TBuilder builder);

    #endregion

    #endregion

    #region (protected) Methods

    protected virtual TBuilder CreateBuilder() => Activator.CreateInstance<TBuilder>();

    #endregion

    //public async Task<int> RunAsync(ICommandLineProgram program, InvocationContext invocationContext)
    //{
    //    try
    //    {
    //        var host = Build(program, invocationContext);

    //        await RunAsync(builder);
    //        return 0;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex);
    //        return 1;
    //    }
    //}


}
