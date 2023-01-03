using LionFire.Execution;
using LionFire.ExtensionMethods.Dumping;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persisters.Expanders;
using LionFire.Referencing;
using LionFire.Vos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using static TestHostBuilder;

namespace ExpanderMounter_.InPlace_;

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
            #region Trying activities
            {
                using var activity = MyActivitySource.StartActivity("SayHello");
                activity?.SetTag("foo", 1);
                activity?.SetTag("bar", "Hello, World!");
                activity?.SetTag("baz", new int[] { 1, 2, 3 });
            }
            #endregion

            var handle = "/testdata/zip/TestTargetDir/TestClass.json".ToVobReference<TestClass>().GetReadHandle<TestClass>();

            var resolveResult = await handle.Resolve().ConfigureAwait(false);
            Assert.IsTrue(resolveResult.IsSuccess);
            Assert.IsTrue(resolveResult.HasValue, "No value");
            Assert.IsNotNull(resolveResult.Value);
            Assert.IsInstanceOfType<RetrieveResult<TestClass>>(resolveResult);
            var r = (RetrieveResult<TestClass>)resolveResult;
            Assert.IsTrue(r.Flags.HasFlag(PersistenceResultFlags.Found), "Not Found");
            Assert.IsFalse(r.Flags.HasFlag(PersistenceResultFlags.NotFound), "Has NotFound flag");
            Assert.AreEqual("Test Name", resolveResult.Value.Name);
            Assert.AreEqual(123, resolveResult.Value.Number);

#warning NEXT: metrics for Vos/file/expand retrieve counts, assert it is the expected number
#warning NEXT: prevent more than 1 read of RZip file


            foreach (var item in ActivitiesExport.Metrics.Value!)
            {
                System.Console.WriteLine($"OTel: {item.Name}: {item.Dump()}");
            }

            sp.GetRequiredService<ILogger<TestLog>>().LogInformation("/".ToVobReference().GetVob().AllMountsRecursive().DumpList("Mounts").ToString());
        });
        Serilog.Log.CloseAndFlush();
    }

    [TestMethod]
    public void _OpenTelemetryTest2()
    {
        H.Run(async sp =>
        {
            Meter Meter = new("LionFire.Test", "1.0");
            Counter<long> TestC = Meter.CreateCounter<long>("TestCounter");
            TestC.Add(1);
            TestC.Add(1);


            //await Task.Delay(3000);
            //TestC.Add(1);
            //await Task.Delay(1200);
            //TestC.Add(1);

            //return Task.CompletedTask;
            //Assert.IsTrue(ActivitiesExport.Metrics.Value!.Count > 0);
            for(int i = 0; i < 30; i++)
            {
                Console.WriteLine("Metrics count: " + ActivitiesExport.Metrics.Value.Count);
                await Task.Delay(100);
            }
            //foreach (var item in ActivitiesExport.Metrics.Value!)
            //{
            //    Debug.WriteLine(" - metric: " + item);
            //}
        });
    }


}


