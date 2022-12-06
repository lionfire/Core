
namespace LionFire.Persisters.Expanders.Tests.ExpansionReadHandle_;

[TestClass]
public class _Resolve
{
    public static IHostBuilder H => TestHostBuilder.H;

    public const string ExpansionReferenceString = "expand:vos://c/TestSourceDir/TestSourceFile.zip:/TestTargetDir/TestTargetFile.txt";


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
