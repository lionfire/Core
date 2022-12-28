using LionFire.Execution;
using LionFire.ExtensionMethods.Dumping;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persisters.Expanders;
using LionFire.Referencing;
using LionFire.Vos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using static TestHostBuilder;

namespace ExpanderMounter_.InPlace_;

public class TestLog { }
[TestClass]
public class _Resolve
{
    [TestMethod]
    public void _AfterList_Temp()
    {
        H.Run(async sp =>
        {
            #region TODO: Get this working without this List
            var listingsHandle = "/testdata/zip".ToVobReference().GetListingsHandle();
            var listings = await listingsHandle.Resolve();
            foreach (var item in listings?.Value.Value ?? Enumerable.Empty<Listing<object>>())
            {
                Debug.WriteLine(item);
            }
            #endregion

            var handle = "/testdata/zip/TestTargetDir/TestClass.json".ToVobReference<TestClass>().GetReadHandle<TestClass>();
            var exists = await handle.Exists().ConfigureAwait(false);
            Assert.IsTrue(exists, "Not found");

            var resolveResult = await handle.Resolve().ConfigureAwait(false);
            Assert.IsTrue(resolveResult.IsSuccess);
            Assert.IsTrue(resolveResult.HasValue);
            Assert.IsNotNull(resolveResult.Value);
            Assert.AreEqual("Test Name", resolveResult.Value.Name);
            Assert.AreEqual(123, resolveResult.Value.Number);
        });
    }

    [TestMethod]
    public void _Exists()
    {
        H.Run(async sp =>
        {
            var handle = "/testdata/zip/TestTargetDir/TestClass.json".ToVobReference<TestClass>().GetReadHandle<TestClass>();

            var exists = await handle.Exists().ConfigureAwait(false);
            sp.GetRequiredService<ILogger<TestLog>>().LogInformation("/".ToVobReference().GetVob().AllMountsRecursive().DumpList("Mounts").ToString());
            Assert.IsTrue(exists, "Not found");
        });
    }

    [TestMethod]
    public void _Resolve_InPlace()
    {
        var MyActivitySource = new ActivitySource("_Resolve_InPlace");

        H.Run(async sp =>
        {
            #region TEMP
            using var activity = MyActivitySource.StartActivity("SayHello");
            activity?.SetTag("foo", 1);
            activity?.SetTag("bar", "Hello, World!");
            activity?.SetTag("baz", new int[] { 1, 2, 3 });
            #endregion

            var handle = "/testdata/zip/TestTargetDir/TestClass.json".ToVobReference<TestClass>().GetReadHandle<TestClass>();
            
            var resolveResult = await handle.Resolve().ConfigureAwait(false);
            Assert.IsTrue(resolveResult.IsSuccess);
            Assert.IsFalse((resolveResult as PersistenceResult)?.Flags.HasFlag(PersistenceResultFlags.NotFound), "Not Found");
            Assert.IsTrue(resolveResult.HasValue, "No value");
            Assert.IsNotNull(resolveResult.Value);
            Assert.AreEqual("Test Name", resolveResult.Value.Name);
            Assert.AreEqual(123, resolveResult.Value.Number);

#warning NEXT: metrics for Vos/file/expand retrieve counts
#warning NEXT: streamline events: instead of BeforeRetrieve, do AfterNotFound

            sp.GetRequiredService<ILogger<TestLog>>().LogInformation("/".ToVobReference().GetVob().AllMountsRecursive().DumpList("Mounts").ToString());
        });
        Serilog.Log.CloseAndFlush();
    }
    
}

