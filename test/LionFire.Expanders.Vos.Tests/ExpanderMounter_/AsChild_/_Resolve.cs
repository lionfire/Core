using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persisters.Expanders;
using LionFire.Referencing;
using LionFire.Vos;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using static TestHostBuilder;

namespace ExpanderMounter_.AsChild_;

[TestClass]
public class _Resolve
{

    [TestMethod]
    public void _()
    {
        H.Run(async sp =>
        {
            #region TODO: Get this working without this List
            var listingsHandle = "/testdata/zip/ExpandAsChildTest".ToVobReference().GetListingsHandle();
            var listings = await listingsHandle.Resolve();
            foreach (var item in listings?.Value.Value ?? Enumerable.Empty<Listing<object>>())
            {
                Debug.WriteLine(item);
            }
            #endregion

            var handle = "/testdata/zip/ExpandAsChildTest/TestTargetDir/TestClass.json".ToVobReference<TestClass>().GetReadHandle<TestClass>();
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
}

