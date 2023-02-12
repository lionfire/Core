using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;

namespace LionFire.Orleans.Cli.SystemCL;

public class DbCommand
{
    public void Options()
    {
        var hostOption = new Option<string>(
            name: "--host",
            description: "address of database server",
            getDefaultValue: () => "localhost");

        var userOption = new Option<string>(
             name: "--user",
             description: "User name for autheneticating to database server",
             getDefaultValue: () => "postgres");

        var passwordOption = new Option<string>(
             name: "--password",
             description: "Password for autheneticating to database server",
             getDefaultValue: () => "localhost");

        var databaseOption = new Option<string?>(
             name: "--database",
             description: "Database name",
             getDefaultValue: () => null);

    }
}
