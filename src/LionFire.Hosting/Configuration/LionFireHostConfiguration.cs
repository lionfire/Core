﻿#nullable enable
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace LionFire.Hosting
{
    public static class HostConfiguration
    {
        //public static IConfigurationRoot CreateDefault(string? basePath = null)
        //    => CreateDefaultBuilder(basePath).Build();

        ///// <summary>
        ///// Get a typical configuration from appsettings.json and environment variables
        ///// </summary>
        ///// <returns></returns>
        //public static IConfigurationBuilder CreateDefaultBuilder(string? basePath = null)
        //    => new ConfigurationBuilder().ConfigureLionFireDefaults(basePath);

        public static IConfigurationBuilder ConfigureLionFireDefaults(this IConfigurationBuilder config, string? basePath = null)
            => config
                   //.SetBasePath(basePath ?? Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? System.IO.Directory.GetCurrentDirectory())
                   //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .If(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) != System.IO.Directory.GetCurrentDirectory(), c =>
                   {
                       System.Console.WriteLine($"Entry location: {Assembly.GetEntryAssembly()?.Location}, CWD: {System.IO.Directory.GetCurrentDirectory()}.  Adding appsettings.json ({File.Exists("appsettings.json").ToString()})");
                        c.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                   }
                   )
            ;
    }
}
