using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;

namespace LionFire.Applications.Hosting
{
    public static class AutoUpgradeAppHostExtensions
    {
        public static IAppHost AutoRestartOnChange<TAssemblyClass>(this IAppHost app)
        {
            var path = typeof(TAssemblyClass).Assembly.Location;

            var pattern = "*.dll";
            Console.WriteLine($"Watching for {pattern} changes at: " + Path.GetDirectoryName(path));
            var fsw = new FileSystemWatcher(Path.GetDirectoryName(path), "*.dll");
            fsw.Changed += Fsw_Changed;
            fsw.Created += Fsw_Changed;

            fsw.EnableRaisingEvents = true;

            return app;
        }

        private static void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("NEW VERSiON AVAILABLE: " + e.FullPath);

            Timer timer = new Timer(20000);
            timer.Elapsed += (sender2, e2) =>
            {
                Console.WriteLine("Killing self");

                Process.GetCurrentProcess().Kill();
            };
            timer.Start();

            //var p = Process.GetCurrentProcess();
            //var mo = p.MainModule.FileName;
            //new ProcessStartInfo(mo, e.FullPath);
        }


    }
}
