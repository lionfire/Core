#nullable enable
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace LionFire.Hosting.CommandLine;

public interface ICommandLineProgram
{
    //RootCommand RootCommand { get; }

    IReadOnlyDictionary<string, IHostingBuilderBuilder> BuilderBuilders { get; }

    TBuilderBuilder GetOrAdd<TBuilderBuilder>(string commandHierarchy)
        where TBuilderBuilder : IHostingBuilderBuilder, new();
    TBuilderBuilder Add<TBuilderBuilder>(string commandHierarchy)
        where TBuilderBuilder : IHostingBuilderBuilder, new();


    Task Handler(InvocationContext context);

    Task<int> RunAsync(string[] args);
}


public static class ICommandLineProgramX
{

    public static IEnumerable<IHostingBuilderBuilder> GetBuilderBuilderHierarchy(this ICommandLineProgram program, InvocationContext context)
    {
        foreach (var command in CommandLineProgramHierarchy.GetCommands(context))
        {
            if (!program.BuilderBuilders.TryGetValue(command, out var builderBuilder))
            {
                //throw new Exception("Missing BuilderBuilder for " + command); // Commenting, since this is probably ok in a lot of cases
            }
            else
            {
                yield return builderBuilder;
                if (!builderBuilder.Inherit) break;
            }
        }
    }
}
