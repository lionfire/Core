using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persisters.Expanders;

namespace ExpansionReference_;

[TestClass]
public class _Exists
{
    public static IHostBuilder H => TestHostBuilder.H;

    static string TestZipPath => Path.Combine(TestHostBuilder.DataDir, "TestSourceFile.zip");
    static string TestZipUrlPath => TestZipPath.Replace(":", "");
    public static readonly string ExpansionReferenceString = $"expand:vos://{TestZipUrlPath}:/TestTargetDir/TestClass.json";

    //public const string ExpansionReferenceString = "expand:vos://c/TestSourceDir/TestSourceFile.zip:/TestTargetDir/TestTargetFile.txt";
    
    [TestMethod]
    public void _()
    {
        H.Run(async sp =>
        {
            var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IExpansionReference>>();

            var expansionReference = ExpansionReferenceString.ToExpansionReference<TestClass>();
            var iExpansionReference = (IExpansionReference<TestClass>)expansionReference;
            var handle = HandleProvider.GetReadHandle(iExpansionReference);

            Assert.AreSame(handle.Reference, iExpansionReference);

            var exists = await handle.Exists();

            Assert.IsTrue(exists);
        });
    }
}
