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
using NLog.MessageTemplates;
using System.Diagnostics;

namespace LionFire.Persisters.SharpZipLib.Tests.Deserialize;

[TestClass]
public class Bytes
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

            var reference = ZipFileReferenceString.ToReference<byte[]>();
            Assert.IsInstanceOfType(reference, typeof(VobReference<byte[]>));
            Assert.AreEqual(expectedPath, reference.Path);

            var readHandle = reference.GetReadHandle<byte[]>();
            Assert.IsInstanceOfType(readHandle, typeof(PersisterReadHandle<IVobReference, byte[], VosPersister>));
            Assert.AreEqual(expectedPath, readHandle.Key);

            var resolveResult = await readHandle.Resolve();
            Assert.IsNotNull(resolveResult.Value);

            Assert.IsTrue(resolveResult.Value.Length > 0);

            #region Unzip

            var ms = new MemoryStream(resolveResult.Value);
            using var zipFile = new ZipFile(ms);

            #endregion

            ZipUtils.ValidateZip(zipFile);
        });
    }

}