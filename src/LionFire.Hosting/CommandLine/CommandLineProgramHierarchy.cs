#nullable enable
using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Text;

namespace LionFire.Hosting;
public static class CommandLineProgramHierarchy
{

    public static IEnumerable<string> GetCommands(InvocationContext context) => GetCommands(GetCommandNames(context));
    public static IEnumerable<string> GetCommands(IEnumerable<string> commands)
    {
        yield return "";
        var sb = new StringBuilder();

        foreach (var command in commands)
        {
            if (sb.Length != 0) sb.Append(' ');
            sb.Append(command);
            yield return sb.ToString();
        }
    }
    public static IEnumerable<string> GetCommands(string commands)
    {
        yield return "";
        var sb = new StringBuilder();

        foreach (var command in commands.Split(" ", StringSplitOptions.RemoveEmptyEntries))
        {
            if (sb.Length != 0) sb.Append(' ');
            sb.Append(command);
            yield return sb.ToString();
        }
    }
  public static IEnumerable<string> GetCommandNames(string commands)
    {
        yield return "";

        foreach (var command in commands.Split(" ", StringSplitOptions.RemoveEmptyEntries))
        {
            yield return command;
        }
    }
    public static IEnumerable<string> GetCommandNames(InvocationContext context)
    {
        var list = new List<string>();
        
        for (CommandResult? commandResult = context.ParseResult.CommandResult; commandResult != null && commandResult.Parent != null; commandResult = commandResult.Parent as CommandResult)
        {
            list.Add(commandResult.Command.Name);
        }

        return list;
    }
}
