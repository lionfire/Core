using System;
using System.Collections.Generic;
using System.Text;
using LionFire.CouchDB;
using LionFire.Data;
using LionFire.Persistence.CouchDB;
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Services
{
    public enum MyCouchMode
    {
        Unspecified = 0,
        Entity = 1,
        Document = 2,
    }
    public static class CouchDBServiceCollectionExtensions
    {
        public static IServiceCollection AddCouchDB(this IServiceCollection services, MyCouchMode mode = MyCouchMode.Entity)
        {
            services
                .AddSingleton<CouchDBPersisterOptions>()

                .AddSingleton<ConnectionManager<CouchDBConnection, CouchDBConnectionOptions>>()

                .AddSingleton<IReadHandleProvider<ICouchDBReference>, PersisterHandleProvider<ICouchDBReference>>()
                .AddSingleton<IReadWriteHandleProvider<ICouchDBReference>, PersisterHandleProvider<ICouchDBReference>>()
                .AddSingleton<IReadHandleProvider<CouchDBReference>, PersisterHandleProvider<CouchDBReference>>()
                .AddSingleton<IReadWriteHandleProvider<CouchDBReference>, PersisterHandleProvider<CouchDBReference>>()
                ;
            switch (mode)
            {
                case MyCouchMode.Unspecified:
                    break;
                case MyCouchMode.Entity:
                    services
                        .AddSingleton<CouchDBEntityPersister>()
                        .AddSingleton<IPersisterProvider<CouchDBReference>, OptionallyNamedPersisterProvider<CouchDBReference, CouchDBEntityPersister, CouchDBPersisterOptions>>()
                        ;
                    break;
                case MyCouchMode.Document:
                    services
                        .AddSingleton<CouchDBSerializingPersister>()
                        .AddSingleton<IPersisterProvider<CouchDBReference>, OptionallyNamedPersisterProvider<CouchDBReference, CouchDBSerializingPersister, CouchDBPersisterOptions>>()
                        ;
                    break;
                default:
                    break;
            }

            return services;
        }
    }
}
