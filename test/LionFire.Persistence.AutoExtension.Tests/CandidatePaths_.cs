using System.Linq;
using LionFire;
using LionFire.Hosting;
using LionFire.Persistence.Filesystem.Tests;
using LionFire.Persistence.Testing;
using LionFire.Services;
using System;
using Xunit;
using LionFire.Vos;
using LionFire.Referencing;
using LionFire.Persistence.Filesystem;
using LionFire.Persistence.AutoExtensionFilesystem;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.Extensions.Options;

namespace CandidatePaths_
{
    public class CandidatePaths_
    {
        [Fact]
        public async void Pass()
        {
            await VosTestHost.Create()
                .ConfigureServices((context, services) => services.AddAutoExtensionFilesystem())
                .RunAsync(async s =>
                {
                    var persister = s.GetRequiredService<AutoExtensionFilesystemPersister>();
                    var vosTest = s.GetRequiredService<VosTest>();
                    var dir = vosTest.VosTestDir;
                    var dirReference = dir.ToFileReference();

                    #region Setup

                    var testString = "testString123";
                    var testBytes = new byte[] { 1, 2, 3 };
                    await "/test/obj1.json".ToVobReference().GetReadWriteHandle<TestClass1>().Set(TestClass1.Create);
                    await "/test/obj1.txt".ToVobReference().GetReadWriteHandle<string>().Set(testString);
                    await "/test/obj1.bin".ToVobReference().GetReadWriteHandle<byte[]>().Set(testBytes);

                    Assert.True(File.Exists(Path.Combine(dir, "obj1.json")));
                    Assert.True(File.Exists(Path.Combine(dir, "obj1.txt")));
                    Assert.True(File.Exists(Path.Combine(dir, "obj1.bin")));

                    #endregion

                    var result = await persister.CandidateReadPaths(Path.Combine(dirReference.Path, "obj1")).ToArrayAsync();
                    Assert.Equal(3, result.Length);
                    Assert.Contains(result, r => r == "json");
                    Assert.Contains(result, r => r == "bin");
                    Assert.Contains(result, r => r == "txt");
                });
        }
    }
}
