using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using LionFire.Hosting;
using LionFire.Execution;
using System.CommandLine.Parsing;
using LionFire.Hosting.CommandLine;

namespace LionFire.Orleans.CommandLine;

public class OrleansCommand
{
    static public Command Create(ICommandLineProgram? program = null)
    {
        var command = new Command("orleans", "manage orleans");

        command.AddCommand(DbCommand.Create(program));

        return command;
    }
}

public class DbCommand
{
    static public IEnumerable<Option> GlobalOptions
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

    static public Command Create(ICommandLineProgram? program = null)
    {
        var dbCommand = new Command("db", "Manage the database");
        foreach (var option in GlobalOptions) dbCommand.AddGlobalOption(option);

        #region Create

        var create = new Command("create", "Create the Orleans database");

        //create.Handler = CommandHandler.Create(() => (program?.HostBuilder.Create() ?? new HostBuilder()).RunCommand(() => Console.WriteLine("TODO: create")));
        create.SetHandler(program.Handler);
    
        dbCommand.AddCommand(create);

        #endregion

        #region Teardown

        var teardown = new Command("teardown", "Delete data in the Orleans database and remove the database from the database server");
        dbCommand.AddCommand(teardown);

        #endregion

        return dbCommand;
    }
}
