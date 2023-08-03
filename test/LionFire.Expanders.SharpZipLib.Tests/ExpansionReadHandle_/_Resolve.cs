using LionFire.Data.Async.Gets;
using LionFire.Testing;

namespace ExpansionReadHandle_;

[TestClass]
public class _Resolve
{
    static string TestZipPath => Path.Combine(TestHostBuilder.DataDir, "TestSourceFile.zip");
    static string TestZipUrlPath => TestZipPath.Replace(":", "");

    public static readonly string ExpansionReferenceString = $"expand:vos://{TestZipUrlPath}:/TestTargetDir/TestTargetFile.txt";
    public static readonly string ExpansionReferenceString_NotFound = $"expand:vos://{TestZipUrlPath}:/TestTargetDir/TestTargetFile-NOTFOUND.txt";
    public static readonly string ExpansionReferenceTestClass = $"expand:vos://{TestZipUrlPath}:/TestTargetDir/TestClass.json";

    [TestMethod]
    public void _String()
    {
        var testZipPath = Path.Combine(TestHostBuilder.DataDir, "TestSourceFile.zip");
        Assert.IsTrue(File.Exists(testZipPath));

        RunTest(async sp =>
        {
            var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IExpansionReference>>();

            var expansionReference = ExpansionReferenceString.ToExpansionReference<string>();
            var iExpansionReference = (IExpansionReference<string>)expansionReference;
            var handle = HandleProvider.GetReadHandle(iExpansionReference);
            Assert.AreSame(handle.Reference, iExpansionReference);

            var exists = await handle.Exists().ConfigureAwait(false);
            Assert.IsTrue(exists);

            var resolveResult = await handle.Get().ConfigureAwait(false);
            Assert.IsTrue(resolveResult.IsSuccess);
            Assert.IsTrue(resolveResult.HasValue);
            Assert.AreEqual("This is a test.", resolveResult.Value);

            #region Metrics

            var metrics = GetMetrics(sp, log: true);
            Assert.AreEqual(2, (long)metrics["LionFire.Vos.Retrieve"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Vos.Retrieve.Batch"].value!);
            Assert.AreEqual(3, (long)metrics["LionFire.Persistence.Handles.WeakHandleRegistry.ReadHandlesCreated"].value!);
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
    public void _String_NotFound()
    {
        RunTest(async sp =>
        {
            var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IExpansionReference>>();

            var expansionReference = ExpansionReferenceString_NotFound.ToExpansionReference<string>();
            var iExpansionReference = (IExpansionReference<string>)expansionReference;
            var handle = HandleProvider.GetReadHandle(iExpansionReference);
            Assert.AreSame(handle.Reference, iExpansionReference);

            var exists = await handle.Exists().ConfigureAwait(false);
            Assert.IsFalse(exists);

            var resolveResult = await handle.Retrieve<string>().ConfigureAwait(false);
            Assert.IsTrue(resolveResult.IsSuccess);
            Assert.IsFalse(resolveResult.HasValue);
            Assert.IsTrue(resolveResult.Flags.HasFlag(TransferResultFlags.NotFound));

            #region Metrics

            var metrics = GetMetrics(sp, log: true);
            Assert.AreEqual(2, (long)metrics["LionFire.Vos.Retrieve"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Vos.Retrieve.Batch"].value!);
            Assert.AreEqual(3, (long)metrics["LionFire.Persistence.Handles.WeakHandleRegistry.ReadHandlesCreated"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persisters.SharpZipLib.StreamRead"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persistence.Filesystem.Exists"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persistence.Filesystem.FileExists"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Persistence.Filesystem.OpenReadStream"].value!);
            //Assert.AreEqual(2, (long)metrics["LionFire.Persisters.SharpZipLib.SharpZipLibExpander.ReadZipFileStream"].value!);
            TestRunner.RanAsserts = true;

            #endregion
        });
    }


    [TestMethod]
    public void _TestClass()
    {
        RunTest(async sp =>
        {
            var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IExpansionReference>>();

            var expansionReference = ExpansionReferenceTestClass.ToExpansionReference<TestClass>();
            var iExpansionReference = (IExpansionReference<TestClass>)expansionReference;
            var handle = HandleProvider.GetReadHandle(iExpansionReference);

            Assert.AreSame(handle.Reference, iExpansionReference);

            var exists = await handle.Exists().ConfigureAwait(false);
            Assert.IsTrue(exists);

            var resolveResult = await handle.Get().ConfigureAwait(false);
            Assert.IsTrue(resolveResult.IsSuccess);
            Assert.IsTrue(resolveResult.HasValue);
            Assert.IsNotNull(resolveResult.Value);
            Assert.AreEqual("Test Name", resolveResult.Value.Name);
            Assert.AreEqual(123, resolveResult.Value.Number);

            #region Metrics

            var metrics = GetMetrics(sp, log: true);
            Assert.AreEqual(2, (long)metrics["LionFire.Vos.Retrieve"].value!);
            Assert.AreEqual(2, (long)metrics["LionFire.Vos.Retrieve.Batch"].value!);
            Assert.AreEqual(3, (long)metrics["LionFire.Persistence.Handles.WeakHandleRegistry.ReadHandlesCreated"].value!);
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
