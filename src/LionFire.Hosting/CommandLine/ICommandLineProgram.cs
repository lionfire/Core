#nullable enable
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting;

public interface ICommandLineProgram
{
    IInitializer<IHostBuilder> ProgramHostBuilderInitializer { get; }
}
