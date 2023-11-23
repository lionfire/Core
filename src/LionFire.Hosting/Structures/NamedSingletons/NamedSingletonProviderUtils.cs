#nullable enable
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace LionFire.Structures
{
    internal static class NamedSingletonProviderUtils<TItem>
    {
        public static Task<TItem> DefaultGet(string key, object[] parameters, NamedSingletonProviderOptions<TItem> options, IServiceProvider? serviceProvider)
        {
            parameters = options.Augment(parameters);

            TItem result;

            if (serviceProvider != null)
            {
                if (parameters != null)
                {
                    result = ActivatorUtilities.CreateInstance<TItem>(serviceProvider, parameters);
                }
                else
                {
                    result = ActivatorUtilities.CreateInstance<TItem>(serviceProvider);
                }
            }
            else
            {
                if (parameters != null)
                {
                    throw new NotImplementedException("parameters can currently only be specified if ServiceProvider is also specified");
                }
                result = Activator.CreateInstance<TItem>();
            }

            TrySetKey(result, key);

            return Task.FromResult(result);
        }

        private static void TrySetKey(TItem obj, string key)
        {
            if (obj is IKeyable keyable)
            {
                keyable.Key = key;
            }
        }
    }
}
