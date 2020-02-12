using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Vos;
using LionFire.Persistence.Filesystem.Tests;
using System.IO;
using LionFire.Persistence.Testing;
using LionFire.Hosting;
using LionFire.Assets;
using LionFire.Referencing;
using LionFire.Services;
using LionFire.Persistence.Filesystem;
using DeepEqual.Syntax;

namespace LionFire.Persistence.Assets.Tests
{
    //public class MyClass
    //{
    //    public string Text { get; set; }
    //    public int Number { get; set; }
    //}

    public class AssetReference_
    {
        [Fact]
        public async void P_GetReadHandle()
        {

            var diskAssetsDir = Path.Combine(FsTestUtils.DataDir, "UnitTestAssets-" + Guid.NewGuid().ToString());

            await AssetTestBuilder.Create()
                 .AddVos()
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddAssets()
                        .AddFilesystem()
                        .AddNewtonsoftJson()
                        .AddAssetPersister() // Default location: root of default vob tree
                        .VosMountRead("/assets", diskAssetsDir.ToFileReference())
                        ;
                })
                .RunAsync(async () =>
              {
                  var path = Path.Combine(diskAssetsDir, "TestClass1", FsTestUtils.TestFileName + ".json");
                  try
                  {

                      #region Setup
                      Directory.CreateDirectory(diskAssetsDir);
                      Directory.CreateDirectory(Path.Combine(diskAssetsDir, "TestClass1"));
                      Assert.False(File.Exists(path));
                      File.WriteAllText(path, TestClass1.ExpectedNewtonsoftJson);

                      #endregion

                      //var assetName = Path.GetFileNameWithoutExtension(path);
                      var assetName = Path.GetFileName(path);

                      AssetReference<TestClass1> mc = assetName;

                      var rh = mc.GetReadHandle<TestClass1>();

                      var result = (await rh.Resolve()).ToRetrieveResult();
                      Assert.True(result.IsSuccess);

                      var value = rh.Value;

                      TestClass1.Create.ShouldDeepEqual(value);
                  }
                  finally
                  {
                      #region Cleanup

                      File.Delete(path);
                      Assert.False(File.Exists(path));
                      Directory.Delete(Path.Combine(diskAssetsDir, "TestClass1"));
                      Directory.Delete(diskAssetsDir);
                      Assert.False(Directory.Exists(diskAssetsDir));

                      #endregion
                  }
              });
        }
    }
}
