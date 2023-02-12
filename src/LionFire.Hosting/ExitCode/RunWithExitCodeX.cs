using System;
using System.Threading.Tasks;

namespace LionFire.Hosting.ExitCode;

public static class RunWithExitCodeX
{
    public static async Task<int> RunWithExitCodeAsync(this string[] args, Func<string[], Task<int>> action)
    {
        try
        {
            return await action(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return 255;
        }
    }
    public static async Task<int> RunWithImpliedExitCodeAsync(this string[] args, Func<string[], Task> action)
    {
        try
        {
            await action(args);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return 255;
        }
    }
    public static int RunWithExitCode(this string[] args, Action<string[]> action)
    {
        try
        {
            action(args);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return 255;
        }
    }
}
