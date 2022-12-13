
namespace ExpansionReadHandle_;

[TestClass]
public class _Resolve
{
    public static IHostBuilder H => TestHostBuilder.H;

    public const string ExpansionReferenceString = "expand:vos://c/Temp/TestSourceDir/TestSourceFile.zip:/TestTargetDir/TestTargetFile.txt";
    public const string ExpansionReferenceString_NotFound = "expand:vos://c/Temp/TestSourceDir/TestSourceFile.zip:/TestTargetDir/TestTargetFile-NOTFOUND.txt";
    public const string ExpansionReferenceTestClass = "expand:vos://c/Temp/TestSourceDir/TestSourceFile.zip:/TestTargetDir/TestClass.json";

    [TestMethod]
    public void _String()
    {
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
            Assert.IsTrue(exists);

            var resolveResult = await handle.Retrieve<string>().ConfigureAwait(false);
            Assert.IsTrue(resolveResult.IsSuccess);
            Assert.IsFalse(resolveResult.HasValue);
            Assert.IsTrue(resolveResult.Flags.HasFlag(PersistenceResultFlags.NotFound));

        });
    }

    // TODO
    //[TestMethod]
    //public void _TestClass()
    //{
    //    H.Run(async sp =>
    //    {
    //        var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IExpansionReference>>();

    //        var expansionReference = ExpansionReferenceTestClass.ToExpansionReference<TestClass>();
    //        var iExpansionReference = (IExpansionReference<TestClass>)expansionReference;
    //        var handle = HandleProvider.GetReadHandle(iExpansionReference);

    //        Assert.AreSame(handle.Reference, iExpansionReference);

    //        var exists = await handle.Exists().ConfigureAwait(false);
    //        Assert.IsTrue(exists);

    //        var exists = await handle.Resolve().ConfigureAwait(false);
    //        Assert.IsTrue(exists);


    //    });
    //}
}
