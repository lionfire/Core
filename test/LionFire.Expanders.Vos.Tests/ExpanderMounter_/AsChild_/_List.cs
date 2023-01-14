using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persisters.Expanders;
using LionFire.Referencing;
using LionFire.Testing;
using LionFire.Vos;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using static TestHostBuilder;

namespace ExpanderMounter_.AsChild_;

[TestClass]
public class _List
{
    [TestMethod]
    public void _List_AsChild()
    {
        RunTest(async sp =>
        {
            var listingsHandle = "/testdata/zip/ExpandAsChildTest".ToVobReference().GetListingsHandle();
            var listings = await listingsHandle.Resolve();

            var listingText = new StringBuilder("+++Listings: ").AppendLine();
            foreach (var item in listings?.Value.Value ?? Enumerable.Empty<Listing<object>>())
            {
                listingText.AppendLine(" - " + item.ToString());
            }
                Log.Logger.Information(listingText.ToString());

            //var handle = "/testdata/zip/ExpandAsChildTest/TestTargetDir/TestClass.json".ToVobReference<TestClass>().GetReadHandle<TestClass>();
            //var exists = await handle.Exists().ConfigureAwait(false);
            //Assert.IsTrue(exists, "Not found");

            //var resolveResult = await handle.TryGetValue().ConfigureAwait(false);
            //Assert.IsTrue(resolveResult.IsSuccess);
            //Assert.IsTrue(resolveResult.HasValue);
            //Assert.IsNotNull(resolveResult.Value);
            //Assert.AreEqual("Test Name", resolveResult.Value.Name);
            //Assert.AreEqual(123, resolveResult.Value.Number);


            #region Metrics

            var metrics = GetMetrics(sp, log: true);
            Assert.AreEqual(12, (long)metrics["LionFire.Persistence.Handles.WeakHandleRegistry.ReadHandlesCreated"].value!);
            Assert.AreEqual(4, (long)metrics["LionFire.Vos.Retrieve"].value!);
            Assert.AreEqual(4, (long)metrics["LionFire.Vos.Retrieve.Batch"].value!);
            Assert.AreEqual(3, (long)metrics["LionFire.Persisters.SharpZipLib.StreamRead"].value!);
            Assert.AreEqual(1698, (long)metrics["LionFire.Persisters.SharpZipLib.StreamReadBytes"].value!);
            Assert.AreEqual(3, (long)metrics["LionFire.Persistence.Filesystem.Exists"].value!);
            Assert.AreEqual(3, (long)metrics["LionFire.Persistence.Filesystem.FileExists"].value!);
            Assert.AreEqual(3, (long)metrics["LionFire.Persistence.Filesystem.OpenReadStream"].value!);
            TestRunner.RanAsserts = true;

            #endregion

            // TODO: Should LionFire.Persisters.SharpZipLib.StreamRead only be 2?

        });
    }
}

