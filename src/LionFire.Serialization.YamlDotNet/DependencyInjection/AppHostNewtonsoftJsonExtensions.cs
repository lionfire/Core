﻿using LionFire.Serialization;
using LionFire.Serialization.Json.Newtonsoft;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LionFire.Dependencies;
using Microsoft.Extensions.Hosting;
using LionFire.Applications.Hosting;
using Newtonsoft.Json;

namespace LionFire.Services
{
    public static class NewtonsoftJsonAppHostExtensions
    {
        public static IAppHost AddNewtonsoftJson(this IAppHost app) => app.TryAddEnumerableSingleton<ISerializationStrategy, NewtonsoftJsonSerializer>();

        public static IHostBuilder AddNewtonsoftJson(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) => services.AddNewtonsoftJson());
            return hostBuilder;
        }

        public static IServiceCollection AddNewtonsoftJson(this IServiceCollection services)
        {
            services
                .TryAddEnumerableSingleton<ISerializationStrategy, NewtonsoftJsonSerializer>()
                .AddSingleton<NewtonsoftJsonSerializer>()
                .Configure<JsonSerializerSettings>(LionSerializeContext.Persistence.ToString(), c => c.SetDefaults(LionSerializeContext.Persistence))
                .Configure<JsonSerializerSettings>(LionSerializeContext.Network.ToString(), c => c.SetDefaults(LionSerializeContext.Network))
                ;
            return services;
        }


        public static JsonSerializerSettings SetDefaults(this JsonSerializerSettings settings, LionSerializeContext context)
        {
            settings.TypeNameHandling = TypeNameHandling.Auto;
            //Converters = ;
            settings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
            //NullValueHandling = 
            settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;

            return settings;
        }
    }
}