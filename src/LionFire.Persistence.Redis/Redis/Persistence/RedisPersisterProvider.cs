using LionFire.Persistence.Persisters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Redis
{
    public class RedisPersisterProvider : OptionallyNamedPersisterProvider<IRedisReference, RedisPersister, RedisPersisterOptions>
    {
        public RedisPersisterProvider(IServiceProvider serviceProvider, IOptionsMonitor<RedisPersisterOptions> options)
            : base(serviceProvider, options)
        { }
    }
}
