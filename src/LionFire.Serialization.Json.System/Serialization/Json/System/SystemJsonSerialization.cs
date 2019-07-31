using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Serialization.Json.System
{

    public class SystemJsonSerialization : IConfigures<IServiceCollection>
    {
        public void Configure(IServiceCollection sc)
        {
            sc.AddSingleton(typeof(ISerializationStrategy), typeof(SystemJsonSerializer));
        }

        //public override  IEnumerable<ServiceDescriptor> ServiceDescriptors
        //{
        //    get
        //    {
        //        yield return new ServiceDescriptor(typeof(ISerializer), typeof(NewtonsoftJsonSerializer), ServiceLifetime.Singleton);
        //    }
        //}

    }
}
