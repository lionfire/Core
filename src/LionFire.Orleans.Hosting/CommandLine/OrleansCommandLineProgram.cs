using LionFire.Orleans.CommandLine;
using Microsoft.Extensions.Hosting;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace LionFire.Hosting;

public static class OrleansCommandLineProgram
{
    public static void AddOrleansCommands(ICommandLineProgram program, RootCommand root)
    {
        root.AddCommand(DbCommand.AddSubcommands(program));
        //var dbCommand = new Command("db", "Database commands")
        //{
        //    Handler = CommandHandler.Create(() => program.ProgramHostBuilderInitializer.Create().Run(() => Console.WriteLine("TODO: db"))),
        //};
    }
}