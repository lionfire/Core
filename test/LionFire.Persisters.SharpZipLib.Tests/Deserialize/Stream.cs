using ICSharpCode.SharpZipLib.Zip;
using LionFire.Hosting;
using LionFire.Persistence;
using LionFire.Persistence.Persisters;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Persisters.SharpZipLib_;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Vos;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace LionFire.Persisters.SharpZipLib.Tests.Deserialize;

[TestClass]
public class Stream_
{
    public static IHostBuilder H => TestHostBuilder.H;
        
    static string TestZipPath => Path.Combine(TestHostBuilder.DataDir, "TestSourceFile.zip");
    static string TestZipUrlPath => TestZipPath.Replace(":", "");

    public static readonly string ZipFileReferenceString = $"vos:///{TestZipUrlPath}";

    // TODO: VirtualFilesystemPersisterBase needs to check whether the Serializer strategy prefers stream (I think a static ctor figures it out?), in which case, don't get byte[], just get a FileStream and pass it to the Serializer

    [TestMethod]
    public void _()
    {
        H.Run(async sp =>
        {
            var expectedPath = "/" + TestZipUrlPath.Replace("\\", "/");

            var reference = ZipFileReferenceString.ToReference<Stream>();
            Assert.IsInstanceOfType(reference, typeof(VobReference<Stream>));
            Assert.AreEqual(expectedPath, reference.Path);

            var readHandle = reference.GetReadHandle<Stream>();
            Assert.IsInstanceOfType(readHandle, typeof(PersisterReadHandle<IVobReference, Stream, VosPersister>));
            Assert.AreEqual(expectedPath, readHandle.Key);

            var resolveResult = await readHandle.Resolve();
            Assert.IsNotNull(resolveResult.Value);

            Assert.IsTrue(resolveResult.Value.Length > 0);

            using var zipFile = new ZipFile(resolveResult.Value);

            ZipUtils.ValidateZip(zipFile);
        });
    }
}
