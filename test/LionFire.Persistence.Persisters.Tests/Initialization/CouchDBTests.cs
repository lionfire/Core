//using LionFire.ObjectBus.Filesystem.Persisters;
using System.IO;
using LionFire.Persistence.Filesystem.Tests;
using LionFire.Persistence.CouchDB;
using Microsoft.Extensions.DependencyInjection;
using LionFire.CouchDB;
using LionFire.Services;
using LionFire.Referencing;
using System;
using LionFire.Persistence;

namespace LionFire
{
    public class CouchDBTests : IPersisterTestInitializer
    {
        public string Scheme => "couch";
        public Type PersisterType => typeof(CouchDBEntityPersister);

        public IServiceCollection AddServicesForTest(IServiceCollection services)
        {
            services.AddCouchDB(MyCouchMode.Entity);
            //services.Configure<CouchDBConnectionOptions>(o => o.ConnectionString = "http://unitTest:unitTestPassword@localhost");
            //services.ConfigureConnection<CouchDBConnectionOptions>(new Uri("http://unitTest:unitTestPassword@localhost"));
            services.ConfigureConnection<CouchDBConnectionOptions>("http://unitTest:unitTestPassword@localhost");
            return services;
        }
        public string GetPathForTestPath(string testPath) => Path.Combine(FsTestUtils.DataDir, testPath);
        public IReference GetReferenceForTestPath(string testPath) => new CouchDBReference(GetPathForTestPath(testPath));
    }
}