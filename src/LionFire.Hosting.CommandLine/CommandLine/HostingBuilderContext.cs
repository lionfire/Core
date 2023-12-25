#nullable enable

using LionFire.FlexObjects;
using LionFire.Hosting.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine.Invocation;
using System.Linq;

namespace LionFire.Hosting;

// ENH: strongly typed for Options type, if possible
public class HostingBuilderBuilderContext : IFlex
{
    object? IFlex.FlexData { get; set; }

    public IHostingBuilderBuilder? HostingBuilderBuilder { get; internal set; }
    public string? CommandHierarchy => HostingBuilderBuilder?.CommandHierarchy;
    public string? CommandName => HostingBuilderBuilder?.Command?.Name;
    public string? InitializingForCommandName { get; set; }

    public IProgram? Program { get; internal set; }
    public InvocationContext? InvocationContext { get; internal set; }

    public Dictionary<string, object?> Options { get; } = new();
    public object? OptionsObject { get; internal set; }
    public T GetOptions<T>() where T : class => (T) OptionsObject!;
    public T? TryGetOptions<T>() where T : class => OptionsObject as T;
}
