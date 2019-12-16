using LionFire.Dependencies;
using LionFire.Serialization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.DependencyInjection.ExtensionMethods
{
    public static class SerializationDependencyInjectionExtensions
    {
            public static IServiceCollection AddBuiltInSerializers(this IServiceCollection services)
            {
                services.AddTextSerializer(); // .txt
                services.AddBinarySerializer(); // .bin
                services.TryAddEnumerableSingleton<ISerializeScorer, MatchingExtensionSerializeScorer>();
                return services;
            }
        }
}
