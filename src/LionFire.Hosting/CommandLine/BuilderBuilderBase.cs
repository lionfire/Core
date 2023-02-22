#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Hosting.CommandLine;

// Supported TBuilder types:
//  - IHostBuilder
//  - HostApplicationBuilder
public abstract class BuilderBuilderBase<TBuilder> : IHostingBuilderBuilder<TBuilder>
{
    public Type BuilderType => typeof(TBuilder);

    public Command Command { get; set; }

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

    public void InitializeHierarchy(ICommandLineProgram program, InvocationContext invocationContext, HostingBuilderBuilderContext context, TBuilder builder)
    {
        foreach (var bb in program.GetBuilderBuilderHierarchy(invocationContext).Cast<IHostingBuilderBuilder<TBuilder>>())
        {
            bb.Initialize(context, builder);
        }

        Initialize(context, builder);
    }

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

    protected virtual TBuilder CreateBuilder() => Activator.CreateInstance<TBuilder>();

    public IHost Build(ICommandLineProgram program, InvocationContext invocationContext)
    {
        var builder = CreateBuilder();

        var context = new HostingBuilderBuilderContext();
        context.AddSingle(program);
        context.AddSingle(invocationContext);

        InitializeHierarchy(program, invocationContext, context, builder);

        return Build(builder);
    }

    public abstract IHost Build(TBuilder builder);
    //protected abstract Task _RunConsoleAsync(TBuilder builder, CancellationToken cancellationToken = default);

    public abstract IHostingBuilderBuilder ConfigureServices(Action<IServiceCollection> services);
}

public class HostApplicationBuilderBuilder : BuilderBuilderBase<HostApplicationBuilder>
{
    public override IHostingBuilderBuilder ConfigureServices(Action<IServiceCollection> services) { Initializers.Add((_, hab) => services(hab.Services)); return this; }
    public override IHost Build(HostApplicationBuilder builder) => builder.Build();
    //protected override Task _RunConsoleAsync(HostApplicationBuilder builder, CancellationToken cancellationToken = default) => builder.Build().RunAsync(cancellationToken);

}

public class HostBuilderBuilder : BuilderBuilderBase<IHostBuilder>
{
    public override IHostingBuilderBuilder ConfigureServices(Action<IServiceCollection> services) { Initializers.Add((_, hb) => hb.ConfigureServices(services)); return this; }
    public override IHost Build(IHostBuilder builder) => builder.Build();
    //protected override Task _RunConsoleAsync(IHostBuilder builder, CancellationToken cancellationToken = default) => builder.Build().RunAsync(cancellationToken);

    protected override IHostBuilder CreateBuilder() => new HostBuilder();

}
