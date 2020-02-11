//using LionFire.ObjectBus.Filesystem.Persisters;
using System.IO;
using LionFire.Persistence.Filesystem.Tests;
using LionFire.Persistence.Filesystem;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Services;
using LionFire.Referencing;
using System;
using LionFire.Persistence;

namespace LionFire
{
    public class FileTests : IPersisterTestInitializer
    {
        public string Scheme => "file";
        public Type PersisterType => typeof(FilesystemPersister);

        public IServiceCollection AddServicesForTest(IServiceCollection services)
        {
            return services
                              .AddFilesystem()
                              ;
        }

        public string GetPathForTestPath(string testPath) => Path.Combine(FsTestUtils.DataDir, testPath);
        public IReference GetReferenceForTestPath(string testPath) => new FileReference(GetPathForTestPath(testPath));

    }
}