using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Applications.Hosting;
using LionFire.Serialization;
using LionFire.Serialization.Json.Newtonsoft;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Applications.Hosting
{
    public static class NewtonsoftJsonAppHostExtensions
    {
        public static IAppHost AddNewtonsoftJson(this IAppHost app)
        {
            app.ServiceCollection.AddSingleton<ISerializationStrategy, NewtonsoftJsonSerializer>();
            return app;
        }
    }
}
