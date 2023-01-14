using LionFire.Dependencies;
using LionFire.Hosting;
using LionFire.Serialization;
using LionFire.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.DependencyInjection.ExtensionMethods;

public static class SerializationDependencyInjectionX
{
        public static IServiceCollection AddBuiltInSerializers(this IServiceCollection services)
        {
            services.AddTextSerializer(); // .txt
            services.AddBinarySerializer(); // .bin
            services.TryAddEnumerableSingleton<ISerializeScorer, MatchingExtensionSerializeScorer>();
            return services;
        }
    }
