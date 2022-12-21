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
//        H.Run(async sp =>
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpansionReadHandle_;

[TestClass]
public class _Exists
{
    public static IHostBuilder H => TestHostBuilder.H;

    static string TestZipPath => Path.Combine(TestHostBuilder.DataDir, "TestSourceFile.zip");
    static string TestZipUrlPath => TestZipPath.Replace(":", "");
    public static readonly string ExpansionReferenceString = $"expand:vos://{TestZipUrlPath}:/TestTargetDir/TestClass.json";

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