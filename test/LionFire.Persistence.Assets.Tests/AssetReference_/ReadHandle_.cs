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

namespace LionFire.Persistence.Assets.Tests
{
    public class MyClass
    {
        public string Text { get; set; }
        public int Number { get; set; }
    }

    public class AssetReference_
    {
        [Fact]
        public async void P_GetReadHandle()
        {
#error NEXT
            await AssetTestBuilder.Create()
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddAssets()
                        .AddAssetPersister() // Default location: root of default vob tree
                        ;
                })
                .RunAsync( () =>
               {
                   var path = FsTestUtils.TestFile + ".json";

                    #region Setup

                    Assert.False(File.Exists(path));
                   File.WriteAllText(path, TestClass1.ExpectedNewtonsoftJson);

                    #endregion

                    var assetName = Path.GetFileNameWithoutExtension(path);

                   AssetReference<MyClass> mc = assetName;

                   var rh = mc.GetReadHandle<MyClass>();


                   Assert.True(true);
               });
        }
    }
}
