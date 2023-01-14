using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persisters.Expanders;
using LionFire.Vos;
using LionFire.Hosting;
using LionFire.Testing;

namespace ExpandMount_;

[TestClass]
public class _TryGetValue
{

    public static readonly string VobReferencePath = $"/test/ExpandMount/TestTargetDir/TestClass.json";

    [TestMethod]
    public void _2()
    {
        RunTest(TestHostBuilder.ExpandMount,
            async sp =>
        {
            var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IVobReference>>();

            var reference = VobReferencePath.ToVobReference<TestClass>();
            var handle = HandleProvider.GetReadHandle(reference);
            Assert.AreEqual(handle.Reference.Path, reference.Path);
            //Assert.IsTrue(ReferenceEquals(handle.Reference, reference));
            //Assert.AreSame(handle.Reference, reference);

            var exists = await handle.Exists().ConfigureAwait(false);
            Assert.IsTrue(exists);

            var resolveResult = await handle.TryGetValue().ConfigureAwait(false);
            Assert.IsTrue(resolveResult.IsSuccess);
            Assert.IsTrue(resolveResult.HasValue);
            Assert.IsNotNull(resolveResult.Value);
            Assert.AreEqual("Test Name", resolveResult.Value.Name);
            Assert.AreEqual(123, resolveResult.Value.Number);

            #region Metrics

            var metrics = GetMetrics(sp, log: true);
            Assert.AreEqual(2, (long)metrics["LionFire.Vos.Retrieve"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Vos.Retrieve.Batch"].value!);
            Assert.AreEqual(4, (long)metrics["LionFire.Persistence.Handles.WeakHandleRegistry.ReadHandlesCreated"].value!);
            Assert.AreEqual(1, (long)metrics["LionFire.Persisters.SharpZipLib.StreamRead"].value!);
            Assert.AreEqual(1, (long)metrics["LionFire.Persistence.Filesystem.Exists"].value!);
            Assert.AreEqual(1, (long)metrics["LionFire.Persistence.Filesystem.FileExists"].value!);
            Assert.AreEqual(1, (long)metrics["LionFire.Persistence.Filesystem.OpenReadStream"].value!);
            Assert.AreEqual(1, (long)metrics["LionFire.Persisters.SharpZipLib.SharpZipLibExpander.ReadZipFileStream"].value!);
            TestRunner.RanAsserts = true;

            #endregion
        });
    }
}
