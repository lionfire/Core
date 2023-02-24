#nullable enable
using System.CommandLine;
using System.Diagnostics;
using System.Linq;
using LionFire.FlexObjects;
using LionFire.Hosting.CommandLine.HostApplicationBuilder_;
using LionFire.Hosting.CommandLine.HostBuilder_;

namespace LionFire.Hosting.CommandLine;

public static class HostBuilderCommandLineProgramX
{
    public static ICommandLineProgram HostBuilder(this ICommandLineProgram program, Action<IHostBuilder> builder, Action<HostBuilderBuilder>? builderBuilder = null) => program.HostBuilder("", builder, builderBuilder: builderBuilder);
    public static ICommandLineProgram HostBuilder(this ICommandLineProgram program, Action<HostingBuilderBuilderContext, IHostBuilder> builder, Action< HostBuilderBuilder>? builderBuilder = null) => program.HostBuilder("", builder, builderBuilder: builderBuilder);
    public static ICommandLineProgram HostBuilder(this ICommandLineProgram program, string commandHierarchy, Action<IHostBuilder> builder, Action<HostBuilderBuilder>? builderBuilder = null) => program.HostBuilder(commandHierarchy, (Action<HostingBuilderBuilderContext, IHostBuilder>)((_, b) => builder(b)), builderBuilder: builderBuilder);
    public static ICommandLineProgram HostBuilder(this ICommandLineProgram program, string commandHierarchy, Action<HostingBuilderBuilderContext, IHostBuilder> builder, Action<HostBuilderBuilder>? builderBuilder = null, Action<Command>? command = null)
    {
        CommandLineProgramValidation.ValidateCommand(commandHierarchy);

        var hostBuilderBuilder = program.GetOrAdd<HostBuilderBuilder>(commandHierarchy);
        builderBuilder?.Invoke(hostBuilderBuilder);
        command?.Invoke(hostBuilderBuilder.Command!);
        hostBuilderBuilder.AddInitializer(builder);

        return program;
    }
}
