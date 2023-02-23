#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Hosting.CommandLine;

// Supported TBuilder types:
//  - IHostBuilder
//  - HostApplicationBuilder
public abstract class BuilderBuilderBase<TBuilder> : IHostingBuilderBuilder<TBuilder>
{
    #region Relationships

    public Command? Command { get; set; }

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
    public List<Action<HostingBuilderBuilderContext, TBuilder>> Initializers { get; set; } = new();

    public void Initialize(HostingBuilderBuilderContext context, TBuilder builder)
    {
        foreach (var init in Initializers)
        {
            init(context, builder);
        }
    }

    #endregion

    #region Methods

    public void InitializeHierarchy(ICommandLineProgram program, InvocationContext invocationContext, HostingBuilderBuilderContext context, TBuilder builder)
    {
        foreach (var bb in program.GetBuilderBuilderHierarchy(invocationContext).Reverse().Cast<IHostingBuilderBuilder<TBuilder>>())
        {
            Debug.WriteLine($"Initializing for {bb.Command.GetType().Name}: " + bb.Command.Name);
            context.InitializingForCommandName = bb.Command.Name;
            try
            {
                bb.Initialize(context, builder);
            }
            finally
            {
                context.InitializingForCommandName = null;
            }
        }
    }

    #region Pass-thru

    public abstract IHostingBuilderBuilder ConfigureServices(Action<IServiceCollection> services);

    public IHost Build(ICommandLineProgram program, InvocationContext invocationContext)
    {
        var builder = CreateBuilder();

        var context = new HostingBuilderBuilderContext
        {
            HostingBuilderBuilder = this,
            Program = program,
            InvocationContext = invocationContext
        };

        InitializeHierarchy(program, invocationContext, context, builder);

        return Build(builder);
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
