using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace LionFire.Hosting.CommandLineOptions
{
    public static class CommandLineOptionsHostingExtensions
    {
        public static IHostBuilder ParseArguments<T>(this IHostBuilder hb, string[] args, Parser parser = null)
            where T : class
        {
            hb.ConfigureServices((context, serviceCollection) => (parser ?? Parser.Default).ParseArguments<T>(args).WithParsed<T>(o => serviceCollection.AddSingleton(o)));
            return hb;
        }
    }
}
