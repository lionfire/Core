using LionFire.Orleans.CommandLine;
using Microsoft.Extensions.Hosting;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace LionFire.Hosting;

public static class OrleansCommandLineProgramX
{
    public static void AddOrleansCommands(this Command root, ICommandLineProgram program)
    {
        // TODO: new 'orleans' command
        // TODO: move db command under command
        root.AddCommand(DbCommand.Create(program));
    }
}