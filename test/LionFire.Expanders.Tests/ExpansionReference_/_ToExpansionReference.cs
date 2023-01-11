using LionFire.Hosting;
using Microsoft.Extensions.Hosting;

namespace LionFire.Persisters.Expanders.Tests.ExpansionReference_;

[TestClass]
public class _ToExpansionReference
{
    public static IHostBuilder H => TestHostBuilder.H;

    public const string ExpansionReferenceString = "expand:vos://c/TestSourceDir/TestSourceFile.zip:/TestTargetDir/TestTargetFile.txt";

    [TestMethod]
    public void _()
    {
        H.Run(sp =>
        {
            var expansionReference = ExpansionReferenceString.ToExpansionReference<TestClass>();

            Assert.IsNotNull(expansionReference);
            Assert.AreEqual("vos://c/TestSourceDir/TestSourceFile.zip", expansionReference.SourceUri);
            Assert.AreEqual("/TestTargetDir/TestTargetFile.txt", expansionReference.Path);
            Assert.AreEqual(typeof(TestClass), expansionReference.Type);

            return Task.CompletedTask;
        });
    }
}
