using LionFire.Persistence;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.Filesystem.Tests;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using LionFire.Serialization;
using LionFire.Hosting;
using LionFire.Referencing;
using LionFire.Services;

namespace FileReadHandle_
{
    public class _Retrieve
    {
        [Fact]
        public async void P_string()
        {
            await PersistersHost.Create()
            .ConfigureServices((_, services) =>
            {
                services.AddFilesystem();
            })
            .RunAsync(async () =>
            {
                var path = FsTestUtils.TestFile + ".txt";
                Assert.False(File.Exists(path));

                FileReference reference = path;
                Assert.Equal(reference.Path, path.Replace('\\', '/'));

                #region Write Test Contents

                var testContents = "testing123";
                File.WriteAllText(path, testContents);
                Assert.True(File.Exists(path));

                #endregion

                #region Retrieve (Primary assertion)

                var readHandle = reference.GetReadHandle<string>();
                //var persistenceResult = await readHandle.Retrieve();
                var persistenceResult = await readHandle.Resolve() as IPersistenceResult;

                Assert.True(persistenceResult.Flags.HasFlag(PersistenceResultFlags.Success));
                Assert.Equal(testContents, readHandle.Value);

                #endregion

                #region Cleanup

                File.Delete(path);
                Assert.False(File.Exists(path));

                #endregion
            });
        }
    }
}
