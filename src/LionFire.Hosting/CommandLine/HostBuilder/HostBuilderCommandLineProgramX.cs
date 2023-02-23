#nullable enable
using System.CommandLine;
using System.Diagnostics;
using System.Linq;
using LionFire.FlexObjects;
using LionFire.Hosting.CommandLine.HostBuilder_;

namespace LionFire.Hosting.CommandLine;

public static class HostBuilderCommandLineProgramX
{
    public static ICommandLineProgram HostBuilder(this ICommandLineProgram program, Action<IHostBuilder> action) => program.HostBuilder("", action);
    public static ICommandLineProgram HostBuilder(this ICommandLineProgram program, string commandHierarchy, Action<IHostBuilder> action, string? description = "") => program.HostBuilder(commandHierarchy, (_, builder) => action(builder), description);
    public static ICommandLineProgram HostBuilder(this ICommandLineProgram program, string commandHierarchy, Action<HostingBuilderBuilderContext, IHostBuilder> initializer, string? description = "")
    {
        CommandLineProgramValidation.ValidateCommand(commandHierarchy);

        var hostBuilderBuilder = program.GetOrAdd<HostBuilderBuilder>(commandHierarchy);
        hostBuilderBuilder.Initializers.Add(initializer);

        return program;
    }
}
