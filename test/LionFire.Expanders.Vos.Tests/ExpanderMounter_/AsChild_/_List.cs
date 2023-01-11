using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persisters.Expanders;
using LionFire.Referencing;
using LionFire.Vos;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Serilog;
using System.Diagnostics;
using static TestHostBuilder;

namespace ExpanderMounter_.AsChild_;

[TestClass]
public class _List
{
    [TestMethod]
    public void _List_AsChild()
    {
        H.Run(async sp =>
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
        });
    }
}

