using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using LionFire.Hosting;

namespace LionFire.Orleans.CommandLine;

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
        var dbCommand = new Command("db", "manage the database");
        foreach (var option in GlobalOptions) dbCommand.AddGlobalOption(option);
        //{ Options = GlobalOptions.ToList() };

        var create = new Command("create", "Create the Orleans database");
        create.Handler = CommandHandler.Create(() => (program?.ProgramHostBuilderInitializer.Create() ?? new HostBuilder()).Run(() => Console.WriteLine("TODO: create")));
        dbCommand.AddCommand(create);

        var teardown = new Command("teardown", "Delete data in the Orleans database and remove the database from the database server");
        dbCommand.AddCommand(teardown);

        return dbCommand;
    }
}
