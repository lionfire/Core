#nullable enable

namespace LionFire.Hosting;

public static class CommandLineProgramValidation
{
    public static void ValidateCommand(string command)
    {
        if (command.Contains("  ")) throw new ArgumentException("command must have single spaces between all subcommands");
    }

}
