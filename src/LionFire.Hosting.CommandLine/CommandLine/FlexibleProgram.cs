#nullable enable

using LionFire;

namespace LionFire.Hosting.CommandLine;

public class FlexibleProgram : CommandLineProgram, IFlexibleProgram
{
    TBuilderBuilder IFlexibleProgram.Add<TBuilderBuilder>(string commandHierarchy)
    {
        return Add<TBuilderBuilder>(commandHierarchy);
    }

    TBuilderBuilder IFlexibleProgram.GetOrAdd<TBuilderBuilder>(string commandHierarchy)
    {
        return GetOrAdd<TBuilderBuilder>(commandHierarchy);
    }
}
