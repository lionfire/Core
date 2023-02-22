#nullable enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Hosting.CommandLine;

public interface IHostingBuilderBuilder
{
    Type BuilderType { get; }
    bool Inherit { get; set; }

    IHost Build(ICommandLineProgram program, InvocationContext invocationContext);
    //Task<int> RunAsync(ICommandLineProgram program, InvocationContext context);

    IHostingBuilderBuilder ConfigureServices(Action<IServiceCollection> services);

    Command Command { get; internal set; }
}

public static class IHostingBuilderBuilderX
{
    public static IHostingBuilderBuilder UseConsoleLifetime(this IHostingBuilderBuilder builder)
    {
        builder.ConfigureServices(services => services.AddSingleton<IHostLifetime, Microsoft.Extensions.Hosting.Internal.ConsoleLifetime>());
        return builder;
    }
    public static IHostingBuilderBuilder UseConsoleLifetime(this IHostingBuilderBuilder builder, Action<ConsoleLifetimeOptions> configureLifetimeOptions)
    {
        builder.ConfigureServices(services => services
            .AddSingleton<IHostLifetime, Microsoft.Extensions.Hosting.Internal.ConsoleLifetime>()
            .Configure(configureLifetimeOptions)
            );
        return builder;
    }

    public static async Task<int> RunAsync(this IHostingBuilderBuilder builder, ICommandLineProgram program, InvocationContext invocationContext, CancellationToken cancellationToken = default)
    {
        try
        {
            await builder
                //.UseConsoleLifetime() // By default, this is already added in CommandLineProgram
                .Build(program, invocationContext)
                .RunAsync(cancellationToken);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return 1;
        }
    }

}

public interface IHostingBuilderBuilder<TBuilder> : IHostingBuilderBuilder
{
    void Initialize(HostingBuilderBuilderContext context, TBuilder builder);
}
