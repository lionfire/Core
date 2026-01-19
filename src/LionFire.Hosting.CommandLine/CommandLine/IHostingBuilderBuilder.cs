#nullable enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Hosting.CommandLine;

public interface IHostingBuilderBuilder
{
    #region Relationships

    IProgram? Program { get; set; }

    Command? Command { get; internal set; }

    Type? OptionsType { get; set; }

    #endregion

    #region Identity

    string CommandHierarchy { get; internal set; }
    
    #endregion

    #region (static) Identity

    Type BuilderType { get; }

    #endregion

    #region Parameters

    bool Inherit { get; set; }

    IHostingBuilderBuilder ConfigureServices(Action<IServiceCollection> services);

    #endregion

    #region Methods

    IHost Build(IProgram program, InvocationContext invocationContext);
    
    #endregion

}

public static class IHostingBuilderBuilderX
{
    public static void AddCommand(this IHostingBuilderBuilder hostingBuilderBuilder, Action<IHostingBuilderBuilder> config)
    {
        config(hostingBuilderBuilder);
    }

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

    public static async Task<int> RunAsync(this IHostingBuilderBuilder builder, IProgram program, InvocationContext invocationContext, CancellationToken cancellationToken = default)
    {
        try
        {
            await builder
                .Build(program, invocationContext)
                .RunAsync(cancellationToken);

            // Respect Environment.ExitCode if set by the hosted task
            return Environment.ExitCode != 0 ? Environment.ExitCode : 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Environment.ExitCode != 0 ? Environment.ExitCode : 1;
        }
    }

}

public interface IHostingBuilderBuilder<TBuilder> : IHostingBuilderBuilder
{
    void Initialize(HostingBuilderBuilderContext context, TBuilder builder);

    void AddInitializer(Action<HostingBuilderBuilderContext, TBuilder> initializer);
}
