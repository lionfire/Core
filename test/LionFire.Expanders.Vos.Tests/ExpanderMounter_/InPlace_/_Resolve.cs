using Google.Protobuf.WellKnownTypes;
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
using System.Globalization;
using System.Runtime.CompilerServices;
using static AnyDiff.DifferenceLines;
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
            //{
            //    using var activity = MyActivitySource.StartActivity("SayHello");
            //    activity?.SetTag("foo", 1);
            //    activity?.SetTag("bar", "Hello, World!");
            //    activity?.SetTag("baz", new int[] { 1, 2, 3 });
            //}
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

            #region Final asserts

            var l = sp.GetRequiredService<ILogger<TestLog>>();
            var mounts = "/".ToVobReference().GetVob().AllMountsRecursive();
            l.LogInformation(mounts.DumpList("Mounts").ToString());
            Assert.AreEqual(3, mounts.Count); // TODO: Metric for total mounts, and/or current moutns

            Assert.AreEqual(1, RootVob.CreateCount);

            #region Metrics

            var metrics = GetMetrics(sp, log: true);

            try
            {
                Assert.AreEqual(2, (long)metrics["LionFire.Vos.Retrieve"].value!);
                Assert.AreEqual(2, (long)metrics["LionFire.Vos.Retrieve.RetryAfterMountsChanged"].value!);
                Assert.AreEqual(2, (long)metrics["LionFire.Vos.Retrieve.Batch"].value!);
                Assert.AreEqual(2, (long)metrics["LionFire.Persistence.Filesystem.Exists"].value!);
                Assert.AreEqual(2, (long)metrics["LionFire.Persistence.Filesystem.FileExistsC"].value!);
                Assert.AreEqual(1, (long)metrics["LionFire.Persistence.Filesystem.DirectoryExistsC"].value!);
                Assert.AreEqual(1, (long)metrics["LionFire.Persistence.Filesystem.OpenReadStream"].value!);
                Assert.AreEqual(1, (long)metrics["LionFire.Persisters.SharpZipLib.StreamRead"].value!);
                Assert.AreEqual(8, metrics.Count);
            }
            catch
            {
                GenerateAsserts(metrics);
                throw;
            }

            #endregion

            #endregion
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
            for (int i = 0; i < 30; i++)
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
