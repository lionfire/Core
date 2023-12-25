#nullable enable
using System.CommandLine;
using System.Linq;


namespace LionFire.Hosting.CommandLine;

public static class RootCommandX
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rootCommand"></param>
    /// <param name="commandHierarchy">A hierarchy of commands, separated by spaces</param>
    /// <returns></returns>
    public static Command GetOrCreateCommandFromHierarchy(this RootCommand rootCommand, string commandHierarchy)
    {
        var hierarchy = CommandLineProgramHierarchy.GetCommandNames(commandHierarchy);
        Command command = rootCommand;

        foreach(var commandName in hierarchy)
        {
            if (commandName == string.Empty) { continue; }

            var child = command.Subcommands.FirstOrDefault(c => c.Name == commandName);
            if (child == null)
            {
                child = new Command(commandName);
                command.AddCommand(child);
            }
            command = child;
        }

        return command;
    }
}

public static class CommandX
{
    #region Add Child commands

    #endregion

}