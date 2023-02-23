#nullable enable
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Hosting.CommandLine;

public interface ICommandLineProgram
{
    IReadOnlyDictionary<string, IHostingBuilderBuilder> BuilderBuilders { get; }

    TBuilderBuilder GetOrAdd<TBuilderBuilder>(string commandHierarchy)
        where TBuilderBuilder : IHostingBuilderBuilder;
    TBuilderBuilder Add<TBuilderBuilder>(string commandHierarchy)
        where TBuilderBuilder : IHostingBuilderBuilder;


    Task<int> Handler(InvocationContext context);

    Task<int> RunAsync(string[] args);
}


public static class ICommandLineProgramX
{

    /// <summary>
    /// </summary>
    /// <param name="program"></param>
    /// <param name="context"></param>
    /// <returns>
    /// Order: from descendant to ancestor
    /// </returns>
    public static IEnumerable<IHostingBuilderBuilder> GetBuilderBuilderHierarchy(this ICommandLineProgram program, InvocationContext context)
    {
        foreach (var command in CommandLineProgramHierarchy.GetCommands(context).Reverse())
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
