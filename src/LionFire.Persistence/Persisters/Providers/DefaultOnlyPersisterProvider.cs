﻿#nullable enable

using LionFire.Referencing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace LionFire.Persistence.Persisters
{
    public class DefaultOnlyPersisterProvider<TReference, TPersister> : PersisterProviderBase<TReference, TPersister>
          where TReference : IReference
        where TPersister : class, IPersister<TReference>
    {
        private TPersister? defaultPersister;

        public override bool HasDefaultPersister => true;

        public DefaultOnlyPersisterProvider(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override TPersister GetPersister(string? name = null)
        {
            if (!string.IsNullOrEmpty(name)) throw new NotSupportedException($"Named persisters not available (Reference tyoe: {typeof(TReference).FullName}, persister type: {typeof(TPersister).FullName})");

            if (defaultPersister == null)
            {
                defaultPersister = ActivatorUtilities.GetServiceOrCreateInstance<TPersister>(serviceProvider);
            }
            return defaultPersister;
        }
    }
}
