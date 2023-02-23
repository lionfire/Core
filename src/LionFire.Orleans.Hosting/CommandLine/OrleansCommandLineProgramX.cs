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

    public static ICommandLineProgram AddOrleansCommands(this ICommandLineProgram program, string commandName = "orleans")
    {
        var orleans = program.Add<HostBuilderBuilder>(commandName);
        orleans.Command.Description = "orleans commands";

        #region db

        var dbCommandName = commandName + " db";
        var db = program.Add<HostBuilderBuilder>(dbCommandName);
        db.Command.Description = "Manage the database";
        foreach (var option in GlobalOptions) db.Command.AddGlobalOption(option);

        #region Create

        var create = program.Add<HostBuilderBuilder>(dbCommandName + " create");
        create.Command.Description = "Create the Orleans database";

        #endregion

        #region Teardown

        var teardown = program.Add<HostBuilderBuilder>(dbCommandName + " teardown");
        teardown.Command.Description = "Delete data in the Orleans database and remove the database from the database server";

        #endregion

        #endregion

        return program;
    }
}

