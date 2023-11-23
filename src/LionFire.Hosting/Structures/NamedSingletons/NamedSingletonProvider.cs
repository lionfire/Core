#nullable enable
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Structures
{
    public class NamedSingletonProvider<TItem> : INamedSingletonProvider<TItem>
    {
        public IServiceProvider? ServiceProvider { get; set; }

        protected NamedSingletonProviderOptions<TItem> Options { get; set; }

        public NamedSingletonProvider(IServiceProvider serviceProvider, IOptionsMonitor<NamedSingletonProviderOptions<TItem>> defaultParameters)
        {
            ServiceProvider = serviceProvider;
            Options = defaultParameters.CurrentValue;
        }

        public virtual Task<TItem> Get(string key, params object[] parameters) 
            => NamedSingletonProviderUtils<TItem>.DefaultGet(key, parameters, Options, ServiceProvider);
    }
}
