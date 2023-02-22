﻿#nullable enable
using Microsoft.Extensions.Hosting;
using System.CommandLine;
using LionFire.FlexObjects;
using System.Runtime.CompilerServices;

namespace LionFire.Hosting.CommandLine;

public static class HostApplicationBuilderCommandLineProgramX
{
    public static ICommandLineProgram HostApplicationBuilder(this ICommandLineProgram program, Action<HostApplicationBuilder> action) => program.HostApplicationBuilder("", action);
    public static ICommandLineProgram HostApplicationBuilder(this ICommandLineProgram program, Action<HostingBuilderBuilderContext, HostApplicationBuilder> action) => program.HostApplicationBuilder("", action);
    public static ICommandLineProgram HostApplicationBuilder(this ICommandLineProgram program, string command, Action<HostApplicationBuilder> action) => program.HostApplicationBuilder(command, (_, builder) => action(builder));
    public static ICommandLineProgram HostApplicationBuilder(this ICommandLineProgram program, string command, Action<HostingBuilderBuilderContext, HostApplicationBuilder> action)
    {
        CommandLineProgramValidation.ValidateCommand(command);

        var hostBuilderBuilder = program.GetOrAdd<HostApplicationBuilderBuilder>(command);
        hostBuilderBuilder.Initializers.Add(action);
        return program;
    }
}

//public class HostApplicationBuilderBuilderBuilder
//{
//    public HostApplicationBuilderBuilderBuilder(HostApplicationBuilderBuilder hostApplicationBuilderBuilder)
//    {
//        HostApplicationBuilderBuilder = hostApplicationBuilderBuilder;
//    }

//    public HostApplicationBuilderBuilder HostApplicationBuilderBuilder { get; }

//    public HostApplicationBuilderBuilderBuilder OnRun(Func<IServiceProvider, Task> run)
//    {
//        HostApplicationBuilderBuilder.Initializers.Add((context, hab) =>
//        {
//            hab.RunAsync(run);
//        });
//    }
//}