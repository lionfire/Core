﻿using LionFire.Execution;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persisters.Expanders;
using LionFire.Referencing;
using LionFire.Vos;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using static TestHostBuilder;

namespace ExpanderMounter_.InPlace_.List_;


[TestClass]
public class _Typed
{
    [TestMethod]
    public void _Typed_NI()
    {
        RunTest(sp =>
        {

            var handle = "/testdata/zip/TestTargetDir/".ToVobReference<TestClass>().GetListingsHandle<TestClass>();

            Assert.ThrowsException<AggregateException>(() =>
            {
                var resolveResult = handle.Get().Result;
                Assert.IsTrue(resolveResult.IsSuccess);
            });

            bool NI = false;
            IGetResult<Metadata<IEnumerable<IListing<TestClass>>>>? resolveResult = null;
            try
            {
                resolveResult = handle.Get().Result;
            }
            catch (AggregateException aex)
            {
                NI = aex.InnerExceptions.OfType<NotImplementedException>().Any();
            }
            Debug.WriteLine(resolveResult);
            Assert.IsTrue(NI);


            //var resolveResult = await handle.Get();
            //Assert.IsTrue(resolveResult.IsSuccess);
            //Assert.IsTrue(resolveResult.HasValue, "No value");
            //Assert.IsNotNull(resolveResult.Value);
            //Assert.IsInstanceOfType<RetrieveResult<Metadata<IEnumerable<IListing<TestClass>>>>>(resolveResult);
            //var r = (RetrieveResult<Metadata<IEnumerable<IListing<TestClass>>>>)resolveResult;
            //Assert.IsTrue(r.Flags.HasFlag(TransferResultFlags.Found), "Not Found");
            //Assert.IsFalse(r.Flags.HasFlag(TransferResultFlags.NotFound), "Has NotFound flag");

            //Assert.AreEqual(1, resolveResult.Value.Value.Count());

            //var first = resolveResult.Value.Value.ElementAt(0);
            //Assert.IsNotNull(first);
            //Assert.AreEqual("TestClass", first.Name);

            //sp.GetRequiredService<ILogger<TestLog>>().LogInformation("/".ToVobReference().GetVob().AllMountsRecursive().DumpList("Mounts").ToString());
        });
        Serilog.Log.CloseAndFlush();
    }
}

