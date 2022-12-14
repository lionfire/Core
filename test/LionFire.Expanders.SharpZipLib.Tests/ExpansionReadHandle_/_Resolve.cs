
namespace ExpansionReadHandle_;

[TestClass]
public class _Resolve
{
    public static IHostBuilder H => TestHostBuilder.H;

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

        H.Run(async sp =>
        {
            var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IExpansionReference>>();

            var expansionReference = ExpansionReferenceString.ToExpansionReference<string>();
            var iExpansionReference = (IExpansionReference<string>)expansionReference;
            var handle = HandleProvider.GetReadHandle(iExpansionReference);
            Assert.AreSame(handle.Reference, iExpansionReference);

            var exists = await handle.Exists().ConfigureAwait(false);
            Assert.IsTrue(exists);

            var resolveResult = await handle.Resolve().ConfigureAwait(false);
            Assert.IsTrue(resolveResult.IsSuccess);
            Assert.IsTrue(resolveResult.HasValue);
            Assert.AreEqual("This is a test.", resolveResult.Value);

        });
    }

    [TestMethod]
    public void _String_NotFound()
    {
        H.Run(async sp =>
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
            Assert.IsTrue(resolveResult.Flags.HasFlag(PersistenceResultFlags.NotFound));

        });
    }


    [TestMethod]
    public void _TestClass()
    {
        H.Run(async sp =>
        {
            var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IExpansionReference>>();

            var expansionReference = ExpansionReferenceTestClass.ToExpansionReference<TestClass>();
            var iExpansionReference = (IExpansionReference<TestClass>)expansionReference;
            var handle = HandleProvider.GetReadHandle(iExpansionReference);

            Assert.AreSame(handle.Reference, iExpansionReference);

            var exists = await handle.Exists().ConfigureAwait(false);
            Assert.IsTrue(exists);

            var resolveResult = await handle.Resolve().ConfigureAwait(false);
            Assert.IsTrue(resolveResult.IsSuccess);
            Assert.IsTrue(resolveResult.HasValue);
            Assert.IsNotNull(resolveResult.Value);
            Assert.AreEqual("Test Name", resolveResult.Value.Name);
            Assert.AreEqual(123, resolveResult.Value.Number);

        });
    }
}
