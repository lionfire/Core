#nullable enable
using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;

namespace LionFire.Hosting;
public static class CommandLineProgramHierarchy
{

    public static IEnumerable<string> GetCommands(InvocationContext context) => GetCommands(GetCommandNames(context).Reverse());

    /// <summary>
    /// 
    /// </summary>
    /// <param name="commandNames">Order: ancestor to descendant</param>
    /// <returns></returns>
    public static IEnumerable<string> GetCommands(IEnumerable<string> commandNames)
    {
        yield return "";
        var sb = new StringBuilder();

        foreach (var command in commandNames)
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

        foreach (var command in commands.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (sb.Length != 0) sb.Append(' ');
            sb.Append(command);
            yield return sb.ToString();
        }
    }
  public static IEnumerable<string> GetCommandNames(string commands)
    {
        yield return "";

        foreach (var command in commands.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
        {
            yield return command;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns>Order: descendant to ancestor</returns>
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
