﻿namespace LionFire.Persisters.Expanders.Tests.ExpansionReference_;

[TestClass]
public class _TryParse
{
    public static IHostBuilder H => TestHostBuilder.H;

    public const string ExpansionReferenceString = "expand:vos://c/TestSourceDir/TestSourceFile.zip:/TestTargetDir/TestTargetFile.txt";

    [TestMethod]
    public void _()
    {
        H.Run(sp =>
        {
            var expansionReference = ExpansionReference.TryParse(ExpansionReferenceString);

            Assert.IsNotNull(expansionReference);
            Assert.AreEqual("vos://c/TestSourceDir/TestSourceFile.zip", expansionReference.SourceKey);
            Assert.AreEqual("/TestTargetDir/TestTargetFile.txt", expansionReference.Path);

            return Task.CompletedTask;
        });
    }

    [TestMethod]
    public void _EscapedColonInTargetPath()
    {
        H.Run(sp =>
        {
            // Target path is probably invalid with a colon in it, but we can support it
            var expansionReference = ExpansionReference.TryParse("expand:vos://c/TestSourceDir:/TestTargetDir/Test::TargetFile.txt");

            Assert.IsNotNull(expansionReference);
            Assert.AreEqual(expansionReference.SourceKey, "vos://c/TestSourceDir");
            Assert.AreEqual(expansionReference.Path, "/TestTargetDir/Test:TargetFile.txt");
            return Task.CompletedTask;
        });
    }

    [TestMethod]
    public void _EscapedColonsInTargetPath()
    {
        H.Run(sp =>
        {
            // Target path is probably invalid with a colon in it, but we can support it
            var expansionReference = ExpansionReference.TryParse("expand:vos://c/TestSourceDir:/TestTargetDir/Test::Targ::etFile.txt");

            Assert.IsNotNull(expansionReference);
            Assert.AreEqual(expansionReference.SourceKey, "vos://c/TestSourceDir");
            Assert.AreEqual(expansionReference.Path, "/TestTargetDir/Test:Targ:etFile.txt");
            return Task.CompletedTask;
        });
    }
}