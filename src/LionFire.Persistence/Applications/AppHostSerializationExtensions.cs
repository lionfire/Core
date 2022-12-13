﻿using System;
using LionFire.Applications.Hosting;
using LionFire.Serialization;
using LionFire.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LionFire.Applications.Hosting
{
    public static class AppHostSerializationExtensions
    {
        public static IServiceCollection AddSerialization(this IServiceCollection services, bool useDefaultSettings = true)
        {
            services
                .AddSingleton<ISerializationProvider, SerializationProvider>()
                .AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptionsMonitor<SerializationOptions>>().CurrentValue)
                ;

            //app.AddSingleton<ISerializationProvider, SerializationProvider>(serviceProvider =>
            //{
            //    var result = new SerializationProvider();
            //    if (useDefaultSettings)
            //    {
            //        result.DeserializationScorers = 
            //    }
            //});

            if (useDefaultSettings)
            {
                foreach (var type in DefaultScorers.DefaultDeserializerScorers)
                {
                    services.TryAddEnumerable(new ServiceDescriptor(typeof(IDeserializeScorer), type, ServiceLifetime.Singleton));
                }
                foreach (var type in DefaultScorers.DefaultSerializerScorers)
                {
                    services.TryAddEnumerable(new ServiceDescriptor(typeof(ISerializeScorer), type, ServiceLifetime.Singleton));
                }
            }
            return services;
        }

        /// <summary>
        /// Registers SerializationProvider in the app's IServicesCollection
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IAppHost AddSerialization(this IAppHost app, bool useDefaultSettings = true)
        {
            app.GenericHost().AddSerialization(useDefaultSettings);

            // OLD
            //app.AddSingleton<ISerializationProvider, SerializationProvider>();
            ////app.AddSingleton<ISerializationProvider, SerializationProvider>(serviceProvider =>
            ////{
            ////    var result = new SerializationProvider();
            ////    if (useDefaultSettings)
            ////    {
            ////        result.DeserializationScorers = 
            ////    }
            ////});

            //if (useDefaultSettings)
            //{
            //    foreach (var type in DefaultScorers.DefaultDeserializerScorers) app.AddSingleton(typeof(IDeserializeScorer), type);
            //    foreach (var type in DefaultScorers.DefaultSerializerScorers) app.AddSingleton(typeof(ISerializeScorer), type);
            //}
            return app;
        }
        public static IHostBuilder AddSerialization(this IHostBuilder hostBuilder, bool useDefaultSettings = true)
            => hostBuilder.ConfigureServices((context, services) => services.AddSerialization());

        /// <summary>
        /// Registers ISerializationService and/or ISerializationStrategy types in the app's IServicesCollection
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IAppHost AddSerializer(this IAppHost app, params Type[] serializationServiceTypes)
        {
            foreach (var type in serializationServiceTypes)
            {
                if (typeof(ISerializationService).IsAssignableFrom(type))
                {
                    app.AddSingleton(typeof(ISerializationService), type);
                }
                else if (typeof(ISerializationStrategy).IsAssignableFrom(type))
                {
                    app.AddSingleton(typeof(ISerializationStrategy), type);
                }
                else
                {
                    throw new ArgumentException("serializationServiceTypes must only contain types assignable to ISerializationService, ISerializationStrategy, orSerializationStrategyPreference");
                }
            }

            return app;
        }

        /// <summary>
        /// Registers SerializationStrategyPreferences in the AppHost
        /// </summary>
        /// <param name="app"></param>
        /// <param name="preferences"></param>
        /// <returns></returns>
        public static IAppHost AddSerializer(this IAppHost app, params SerializationStrategyPreference[] preferences)
        {
            foreach (var preference in preferences) app.Add(preference);
            return app;
        }

        /// <summary>
        /// Registers ISerializationStrategy in the app's IServicesCollection
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IAppHost AddSerializationStrategy<T>(this IAppHost app)
            where T : ISerializationStrategy, new() => app.AddSerializer(typeof(T));

        /// <summary>
        /// Registers ISerializationService in the app's IServicesCollection
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IAppHost AddSerializationService<T>(this IAppHost app)
            where T : ISerializationService, new()
        {
            app.Add(new SerializationStrategyPreference(new T()));
            return app;
        }
    }
}
