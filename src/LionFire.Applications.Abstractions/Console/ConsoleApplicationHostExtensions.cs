using LionFire.Applications.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Applications
{
    public static class ConsoleApplicationHostExtensions
    {
        public static IAppHost AddShutdownOnConsoleExitCommand(this IAppHost app, bool waitForCompletion = false)
        {
            app.Add(
                new AppTask(
                    () =>
                    {
                        var exit = "exit";
                        while (Console.ReadLine() != "exit")
                        {
                            Console.WriteLine($"Type {exit} to close the program.");
                        }

                        Console.WriteLine("Shutting down...");

                        app.Shutdown();
                    })
                {
                    WaitForCompletion = waitForCompletion
                });
            return app;
        }
    }
}
