using ICSharpCode.SharpZipLib.Zip;
using LionFire.Hosting;
using LionFire.Persistence.Persisters;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Persisters.SharpZipLib_;
using LionFire.Referencing;
using LionFire.Vos;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace LionFire.Persisters.SharpZipLib.Tests.Deserialize;

[TestClass]
public class Zip
{
    public static IHostBuilder H => TestHostBuilder.H;

    static string TestZipPath => Path.Combine(TestHostBuilder.DataDir, "TestSourceFile.zip");
    static string TestZipUrlPath => TestZipPath.Replace(":", "");

    public static readonly string ZipFileReferenceString = $"vos:///{TestZipUrlPath}";

    [TestMethod]
    public void _()
    {
        H.Run(async sp =>
        {
            var expectedPath = "/" + TestZipUrlPath.Replace("\\", "/");

            var reference = ZipFileReferenceString.ToReference<ZipFile>();
            Assert.IsInstanceOfType(reference, typeof(VobReference<ZipFile>));
            Assert.AreEqual(expectedPath, reference.Path);

            var readHandle = reference.GetReadHandle<ZipFile>();
            Assert.IsInstanceOfType(readHandle, typeof(PersisterReadHandle<IVobReference, ZipFile, VosPersister>));
            Assert.AreEqual(expectedPath, readHandle.Key);

            var resolveResult = await readHandle.Resolve();
            Assert.IsNotNull(resolveResult.Value);

            using var zipFile = resolveResult.Value;

            ZipUtils.ValidateZip(zipFile);
        });
    }
}
