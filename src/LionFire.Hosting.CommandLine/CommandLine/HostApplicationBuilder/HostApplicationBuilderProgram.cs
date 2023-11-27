using LionFire.Hosting.CommandLine.HostApplicationBuilder_;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting.CommandLine;

public class HostApplicationBuilderProgram : CommandLineProgram<HostApplicationBuilder, HostApplicationBuilderBuilder>
{
    public bool DisableDefaults { get; set; } = false;

    public HostApplicationBuilderProgram(bool disableDefaults = false)
    {
        DisableDefaults = disableDefaults;
    }
}

public static class HostApplicationBuilderProgramX
{
    public static HostApplicationBuilderProgram DisableDefaults(this HostApplicationBuilderProgram p)
    {
        p.DisableDefaults = true;
        return p;
    }
}