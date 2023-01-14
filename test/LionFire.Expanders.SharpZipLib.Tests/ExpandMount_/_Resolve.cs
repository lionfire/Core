using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persisters.Expanders;
using LionFire.Vos;
using LionFire.Hosting;
using LionFire.Resolves;
using LionFire.Testing;

namespace ExpandMount_;

[TestClass]
public class _Resolve
{
    public static readonly string VobReferencePath = $"/test/ExpandMount/TestTargetDir/TestClass.json";

    public Action<HostApplicationBuilder> config => TestHostBuilder.ExpandMount;

    [TestMethod]
    public void _1()
    {
        RunTest(config, async sp =>
        {
            var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IVobReference>>();

            var reference = VobReferencePath.ToVobReference<TestClass>();
            var handle = HandleProvider.GetReadHandle(reference);
            Assert.AreEqual(handle.Reference.Path, reference.Path);
            //Assert.IsTrue(ReferenceEquals(handle.Reference, reference));
            //Assert.AreSame(handle.Reference, reference);

            var resolveResult = await handle.Resolve().ConfigureAwait(false);
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

    [TestMethod]
    public void _AfterExists()
    {
        RunTest(config, async sp =>
        {
            var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IVobReference>>();

            var reference = VobReferencePath.ToVobReference<TestClass>();
            var handle = HandleProvider.GetReadHandle(reference);
            Assert.AreEqual(handle.Reference.Path, reference.Path);
            //Assert.IsTrue(ReferenceEquals(handle.Reference, reference));
            //Assert.AreSame(handle.Reference, reference);

            var exists = await handle.Exists().ConfigureAwait(false);
            Assert.IsTrue(exists);

            var resolveResult = await handle.Resolve().ConfigureAwait(false);
            Assert.IsTrue(resolveResult.IsSuccess);
            Assert.IsTrue(resolveResult.HasValue);
            Assert.IsNotNull(resolveResult.Value);
            Assert.AreEqual("Test Name", resolveResult.Value.Name);
            Assert.AreEqual(123, resolveResult.Value.Number);

            #region Metrics

            var metrics = GetMetrics(sp, log: true);
            Assert.AreEqual(4, (long)metrics["LionFire.Vos.Retrieve"].value!);
            Assert.AreEqual(4, (long)metrics["LionFire.Vos.Retrieve.Batch"].value!);
            Assert.AreEqual(4, (long)metrics["LionFire.Persistence.Handles.WeakHandleRegistry.ReadHandlesCreated"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persisters.SharpZipLib.StreamRead"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persistence.Filesystem.Exists"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persistence.Filesystem.FileExists"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persistence.Filesystem.OpenReadStream"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persisters.SharpZipLib.SharpZipLibExpander.ReadZipFileStream"].value!);
            TestRunner.RanAsserts = true;

            #endregion

        });
    }

    [TestMethod]
    public void _Twice()
    {
        RunTest(config, async sp =>
        {
            var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IVobReference>>();

            var reference = VobReferencePath.ToVobReference<TestClass>();
            var handle = HandleProvider.GetReadHandle(reference);
            Assert.AreEqual(handle.Reference.Path, reference.Path);
            //Assert.IsTrue(ReferenceEquals(handle.Reference, reference));
            //Assert.AreSame(handle.Reference, reference);

            void validate(IResolveResult<TestClass> resolveResult)
            {
                Assert.IsTrue(resolveResult.IsSuccess);
                Assert.IsTrue(resolveResult.HasValue);
                Assert.IsNotNull(resolveResult.Value);
                Assert.AreEqual("Test Name", resolveResult.Value.Name);
                Assert.AreEqual(123, resolveResult.Value.Number);
            }

            validate(await handle.Resolve().ConfigureAwait(false));
            validate(await handle.Resolve().ConfigureAwait(false));

            #region Metrics

            var metrics = GetMetrics(sp, log: true);
            Assert.AreEqual(4, (long)metrics["LionFire.Vos.Retrieve"].value!);
            Assert.AreEqual(4, (long)metrics["LionFire.Vos.Retrieve.Batch"].value!);
            Assert.AreEqual(4, (long)metrics["LionFire.Persistence.Handles.WeakHandleRegistry.ReadHandlesCreated"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persisters.SharpZipLib.StreamRead"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persistence.Filesystem.Exists"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persistence.Filesystem.FileExists"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persistence.Filesystem.OpenReadStream"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persisters.SharpZipLib.SharpZipLibExpander.ReadZipFileStream"].value!);
            TestRunner.RanAsserts = true;

            #endregion

        });
    }

    [TestMethod]
    public void _Discard_ResolveAgain()
    {
        RunTest(config, async sp =>
        {
            var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IVobReference>>();

            var reference = VobReferencePath.ToVobReference<TestClass>();
            var handle = HandleProvider.GetReadHandle(reference);
            Assert.AreEqual(handle.Reference.Path, reference.Path);
            //Assert.IsTrue(ReferenceEquals(handle.Reference, reference));
            //Assert.AreSame(handle.Reference, reference);

            void validate(IResolveResult<TestClass> resolveResult)
            {
                Assert.IsTrue(resolveResult.IsSuccess);
                Assert.IsTrue(resolveResult.HasValue);
                Assert.IsNotNull(resolveResult.Value);
                Assert.AreEqual("Test Name", resolveResult.Value.Name);
                Assert.AreEqual(123, resolveResult.Value.Number);
            }

            validate(await handle.Resolve().ConfigureAwait(false));
            handle.DiscardValue();
            validate(await handle.Resolve().ConfigureAwait(false));

            #region Metrics

            var metrics = GetMetrics(sp, log: true);
            Assert.AreEqual(4, (long)metrics["LionFire.Vos.Retrieve"].value!);
            Assert.AreEqual(4, (long)metrics["LionFire.Vos.Retrieve.Batch"].value!);
            Assert.AreEqual(4, (long)metrics["LionFire.Persistence.Handles.WeakHandleRegistry.ReadHandlesCreated"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persisters.SharpZipLib.StreamRead"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persistence.Filesystem.Exists"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persistence.Filesystem.FileExists"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persistence.Filesystem.OpenReadStream"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persisters.SharpZipLib.SharpZipLibExpander.ReadZipFileStream"].value!);
            TestRunner.RanAsserts = true;

            #endregion

        });
    }
}
