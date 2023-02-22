using LionFire.Hosting.CommandLine;
using LionFire.Orleans.CommandLine;
using Microsoft.Extensions.Hosting;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace LionFire.Hosting;

public static class OrleansCommandLineProgramX
{
    public static ICommandLineProgram AddOrleansCommands(this ICommandLineProgram program)
    {
        program.Add<HostBuilderBuilder>("orleans");

        // TODO NEXT
        //program.RootCommand.AddCommand(OrleansCommand.Create(program));
        return program;
    }
}