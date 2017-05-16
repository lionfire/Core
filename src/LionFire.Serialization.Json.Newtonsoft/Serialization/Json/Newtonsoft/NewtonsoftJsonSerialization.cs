using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Serialization.Json.Newtonsoft
{

    public class NewtonsoftJsonSerialization : AppPackage
    {
        public override void ConfigureServices(IServiceCollection sc)
        {
            sc.AddSingleton(typeof(ISerializer), typeof(NewtonsoftJsonSerializer));
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
