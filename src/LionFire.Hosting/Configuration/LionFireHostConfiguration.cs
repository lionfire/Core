#nullable enable
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;

namespace LionFire.Hosting;

public static class HostConfiguration
{
    // REVIEW - OLD?
    [Obsolete("UNUSED?")]
    public static IConfigurationBuilder ConfigureLionFireDefaults(this IConfigurationBuilder config, string? basePath = null)
        => config
               //.SetBasePath(basePath ?? Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? System.IO.Directory.GetCurrentDirectory())
               //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .If(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) != System.IO.Directory.GetCurrentDirectory(), c =>
               {
                   System.Console.WriteLine($"Entry location: {Assembly.GetEntryAssembly()?.Location}, CWD: {System.IO.Directory.GetCurrentDirectory()}.  Adding appsettings.json (exists: {File.Exists("appsettings.json").ToString()})");
                    c.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
               }
               )
        ;
}
