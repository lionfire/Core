#nullable enable

using System.Linq;

namespace LionFire.Hosting;

public class HostingBuilderBuilderContext : IFlex 
{
    object? IFlex.FlexData { get; set; }

    public string[] CommandHierarchy { get; set; }
    public string CommandName => CommandHierarchy?.LastOrDefault();
}
