#nullable enable

namespace LionFire.Hosting.CommandLine;

public interface IFlexibleProgram : IProgram
{
    TBuilderBuilder GetOrAdd<TBuilderBuilder>(string commandHierarchy)
        where TBuilderBuilder : IHostingBuilderBuilder;
    TBuilderBuilder Add<TBuilderBuilder>(string commandHierarchy)
        where TBuilderBuilder : IHostingBuilderBuilder;
}
