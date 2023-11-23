#nullable enable
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using LionFire.FlexObjects;
using System.Runtime.CompilerServices;
using LionFire.Hosting.CommandLine.HostApplicationBuilder_;

namespace LionFire.Hosting.CommandLine;

public static class HostApplicationBuilderFlexibleProgramX
{
    public static IFlexibleProgram HostApplicationBuilder(this IFlexibleProgram program, Action<HostApplicationBuilder> buildBuilderAction, Action<HostApplicationBuilderBuilder>? configureBuilderBuilder = null) => program.HostApplicationBuilder("", buildBuilderAction, configureBuilderBuilder: configureBuilderBuilder);
    public static IFlexibleProgram HostApplicationBuilder(this IFlexibleProgram program, Action<HostingBuilderBuilderContext, HostApplicationBuilder> buildBuilderAction, Action<HostApplicationBuilderBuilder>? configureBuilderBuilder = null) => program.HostApplicationBuilder("", buildBuilderAction, configureBuilderBuilder: configureBuilderBuilder);
    public static IFlexibleProgram HostApplicationBuilder(this IFlexibleProgram program, string commandHierarchy, Action<HostApplicationBuilder> buildBuilderAction, Action<HostApplicationBuilderBuilder>? configureBuilderBuilder = null) => program.HostApplicationBuilder(commandHierarchy, (_, builder) => buildBuilderAction(builder), configureBuilderBuilder: configureBuilderBuilder);
    public static IFlexibleProgram HostApplicationBuilder(this IFlexibleProgram program, string commandHierarchy, Action<HostingBuilderBuilderContext, HostApplicationBuilder> buildBuilderAction, Action<HostApplicationBuilderBuilder>? configureBuilderBuilder = null, Action<Command>? command = null)
    {
        CommandLineProgramValidation.ValidateCommand(commandHierarchy);

        var hostBuilderBuilder = program.GetOrAdd<HostApplicationBuilderBuilder>(commandHierarchy);
        configureBuilderBuilder?.Invoke(hostBuilderBuilder);
        command?.Invoke(hostBuilderBuilder.Command!);
        hostBuilderBuilder.AddInitializer(buildBuilderAction);
        return program;
    }
}
