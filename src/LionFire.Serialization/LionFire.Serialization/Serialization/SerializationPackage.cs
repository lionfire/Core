using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Serialization
{
    public class SerializationPackage : IConfigures<IServiceCollection>
    {
        public void Configure(IServiceCollection sc)
        {
            sc.AddSingleton<IFileDeserializer, FileDeserializer>();
        }
    }
}
