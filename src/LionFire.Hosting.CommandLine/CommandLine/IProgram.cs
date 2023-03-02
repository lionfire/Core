#nullable enable
using System.CommandLine.Invocation;

namespace LionFire.Hosting.CommandLine;

public interface IProgram
{
    IReadOnlyDictionary<string, IHostingBuilderBuilder> BuilderBuilders { get; }


    Task<int> Handler(InvocationContext context);

    Task<int> RunAsync(string[] args);
}
