#nullable enable

using LionFire.Hosting.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine.Invocation;
using System.Linq;

namespace LionFire.Hosting;

public class HostingBuilderBuilderContext : IFlex
{
    object? IFlex.FlexData { get; set; }

    public IHostingBuilderBuilder? HostingBuilderBuilder { get; internal set; }
    public string? CommandHierarchy => HostingBuilderBuilder?.CommandHierarchy;
    public string? CommandName => HostingBuilderBuilder?.Command.Name;
    public string? InitializingForCommandName { get; set; }

    public ICommandLineProgram? Program { get; internal set; }
    public InvocationContext? InvocationContext { get; internal set; }

}
