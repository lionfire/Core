#nullable enable
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Linq;

namespace LionFire.Hosting.CommandLine;

public static class IProgramX
{

    /// <summary>
    /// </summary>
    /// <param name="program"></param>
    /// <param name="context"></param>
    /// <returns>
    /// Order: from descendant to ancestor
    /// </returns>
    public static IEnumerable<IHostingBuilderBuilder> GetBuilderBuilderHierarchy(this IProgram program, InvocationContext context)
    {
        foreach (var commandHierarchy in CommandLineProgramHierarchy.GetCommands(context).Reverse())
        {
            if (!program.BuilderBuilders.TryGetValue(commandHierarchy, out var builderBuilder))
            {
                //throw new Exception("Missing BuilderBuilder for " + command); // Commenting, since this is probably ok in a lot of cases
                Debug.WriteLine("Missing BuilderBuilder for " + commandHierarchy); // Commenting, since this is probably ok in a lot of cases
            }
            else
            {
                yield return builderBuilder;
                if (!builderBuilder.Inherit) break;
            }
        }
    }

    public static T DefaultArgs<T>(this T program, string[] args)
        where T : IProgram
    {
        program.DefaultArgsList = args;
        return program;
    }
    public static T DefaultArgs<T>(this T program, string args)
        where T : IProgram
    {
        if (args.Contains('"'))
        {
            throw new NotImplementedException("Not implemented yet: doublequotes");
        }

        program.DefaultArgsList = args.Split(' ');
        return program;
    }
}
