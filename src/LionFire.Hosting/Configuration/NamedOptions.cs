using LionFire.ExtensionMethods;
using LionFire.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace LionFire.Configuration
{

    public class NamedOptions<TOptions>
        where TOptions : new()
    {
        public Dictionary<string, TOptions> OptionsByName { get; } = new Dictionary<string, TOptions>();

        public TOptions GetOptionsOrDefault(string name) => OptionsByName.TryGetValue(name);

        public TOptions this[string name] => GetOptionsOrDefault(name);
    }

    public static class NamedOptionsExtensions
    {
        public static LionFireHostBuilder NamedOptions<TOptions>(this LionFireHostBuilder builder, string configRootKey)
            where TOptions : new()
            => builder.ForHostBuilder(b=>b
                    .ConfigureServices((context, services) => services
                    .Configure<NamedOptions<TOptions>>(o =>
                    {
                        var config = context.Configuration;
                        if(configRootKey != null)
                        {
                            config = config.GetSection(configRootKey);
                        }

                        foreach(var childSection in config.GetChildren())
                        {
                            //ConfigurationBinder
                            var child = new TOptions();
                            childSection.Bind(child);
                            o.OptionsByName.Add(childSection.Key, child);
                        }
                    })));
    }
}
