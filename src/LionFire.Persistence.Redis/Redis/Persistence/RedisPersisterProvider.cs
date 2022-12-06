using LionFire.Persistence.Persisters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Redis
{
    public class RedisPersisterProvider : OptionallyNamedPersisterProvider<IRedisReference, RedisPersister>
    {
        public RedisPersisterProvider(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }
    }
}
