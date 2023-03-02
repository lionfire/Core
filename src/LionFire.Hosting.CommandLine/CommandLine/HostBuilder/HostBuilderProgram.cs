#nullable enable

using LionFire;
using LionFire.Hosting.CommandLine.HostBuilder_;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting.CommandLine;

public class HostBuilderProgram : CommandLineProgram<IHostBuilder, HostBuilder_.HostBuilderBuilder>
{
  
}
