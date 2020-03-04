using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Serialization.Json.JsonEx
{
    public class JsonExSerialization : IConfigures<IServiceCollection>
    {
        public void Configure(IServiceCollection services) => services.AddSingleton(typeof(ISerializationStrategy), typeof(JsonExLionFireSerializer));
    }
}
