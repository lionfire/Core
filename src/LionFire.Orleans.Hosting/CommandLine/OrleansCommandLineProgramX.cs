using LionFire.Hosting.CommandLine;
using LionFire.Hosting.CommandLine.HostBuilder_;
using System.Collections.Generic;
using System.CommandLine;

namespace LionFire.Hosting;

public static class OrleansCommandLineProgramX
{
    public static IEnumerable<Option> GlobalOptions
    {
        get
        {
            var hostOption = new Option<string>(
                name: "--host",
                description: "address of database server",
                getDefaultValue: () => "localhost");
            yield return hostOption;

            var userOption = new Option<string>(
                 name: "--user",
                 description: "User name for autheneticating to database server",
                 getDefaultValue: () => "postgres");
            yield return userOption;

            var passwordOption = new Option<string>(
                 name: "--password",
                 description: "Password for autheneticating to database server",
                 getDefaultValue: () => "localhost");
            yield return passwordOption;

            var databaseOption = new Option<string?>(
                 name: "--database",
                 description: "Database name",
                 getDefaultValue: () => null);
            yield return databaseOption;
        }
    }

    public static CommandLineProgram<TBuilder, TBuilderBuilder> AddOrleansCommands<TBuilder, TBuilderBuilder>(this CommandLineProgram<TBuilder, TBuilderBuilder> program, string commandName = "orleans")
        where TBuilderBuilder : IHostingBuilderBuilder<TBuilder>
    {
        var orleans = program.Command(commandName, command: c => c.Description = "orleans commands");

        #region db

        var dbCommandName = commandName + " db";
        var db = program.Command(dbCommandName,
            //(c,b) => c.HostingBuilderBuilder.ConfigureServices(s => s.AddRunTaskAndStop(() => Console.WriteLine("TODO: orleans db NOOP"))),
            //bb=>bb.Inherit = false,
            command: c =>
            {
                c.Description = "Manage the database";
                foreach (var option in GlobalOptions) c.AddGlobalOption(option);
            }
            );

        #region Create

        var create = program.Command(dbCommandName + " create",
            (c,b) => c.HostingBuilderBuilder.ConfigureServices(s => s.AddRunTaskAndStop(() => Console.WriteLine("TODO: orleans db create"))),
            command: c => c.Description = "Create the Orleans database");

        #endregion

        #region Teardown

        var teardown = program.Command(dbCommandName + " teardown",
           (c, b) => c.HostingBuilderBuilder.ConfigureServices(s => s.AddRunTaskAndStop(() => Console.WriteLine("TODO: orleans db teardown"))),
           command: c => c.Description = "Delete data in the Orleans database and remove the database from the database server");

        #endregion

        #endregion

        return program;
    }
}

