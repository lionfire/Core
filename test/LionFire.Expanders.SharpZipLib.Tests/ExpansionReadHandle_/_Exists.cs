//using LionFire.Persistence;
//using LionFire.Persistence.Handles;
//using LionFire.Persisters.Expanders;
//using LionFire.Vos;

//namespace ExpansionReference_;

//[TestClass]
//public class _Exists
//{
//    public static IHostBuilder H => TestHostBuilder.ExpandMount;

//    public static readonly string VobReferenceString = $"vos:///test/ExpandMount/TestTargetDir/TestClass.json";
    
//    [TestMethod]
//    public void _()
//    {
//        RunTest(async sp =>
//        {
//            var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IExpansionReference>>();

//            var reference = VobReferenceString.ToVobReference<TestClass>();
//            var handle = HandleProvider.GetReadHandle(reference);
//            Assert.AreSame(handle.Reference, reference);

//            var exists = await handle.Exists();

//            Assert.IsTrue(exists);

//        });
//    }
//}


using LionFire.Persistence.Handles;
using LionFire.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpansionReadHandle_;

[TestClass]
public class _Exists
{
    static string TestZipPath => Path.Combine(TestHostBuilder.DataDir, "TestSourceFile.zip");
    static string TestZipUrlPath => TestZipPath.Replace(":", "");
    public static readonly string ExpansionReferenceString = $"expand:vos://{TestZipUrlPath}:/TestTargetDir/TestClass.json";

    [TestMethod]
    public void _()
    {
        RunTest(async sp =>
        {
            var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IExpansionReference>>();

            var expansionReference = ExpansionReferenceString.ToExpansionReference<TestClass>();
            var iExpansionReference = (IExpansionReference<TestClass>)expansionReference;
            var handle = HandleProvider.GetReadHandle(iExpansionReference);

            Assert.AreSame(handle.Reference, iExpansionReference);

            var exists = await handle.Exists();
            Assert.IsTrue(exists);

            #region Metrics

            var metrics = GetMetrics(sp, log: true);
            Assert.AreEqual(1, (long)metrics["LionFire.Vos.Retrieve"].value!);
            Assert.AreEqual(1, (long)metrics["LionFire.Vos.Retrieve.Batch"].value!);
            Assert.AreEqual(3, (long)metrics["LionFire.Persistence.Handles.WeakHandleRegistry.ReadHandlesCreated"].value!);
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