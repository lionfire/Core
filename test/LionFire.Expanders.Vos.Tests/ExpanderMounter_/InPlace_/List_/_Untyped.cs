using LionFire.ExtensionMethods.Dumping;
using LionFire.Resolves;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using static TestHostBuilder;

namespace ExpanderMounter_.InPlace_.List_;

[TestClass]
public class _Untyped
{

    [TestMethod]
    public void _() => _untyped("/testdata/zip/TestTargetDir");
    [TestMethod]
    public void _TrailingSlash() => _untyped("/testdata/zip/TestTargetDir/");

    private void _untyped(string path)
    {
        H.Run(async sp =>
        {
            var handle = path.ToVobReference<TestClass>().GetListingsHandle<object>();

            IResolveResult<Metadata<IEnumerable<Listing<object>>>> resolveResult = await handle.Resolve().ConfigureAwait(false);
            ValidateListing(resolveResult);

            #region Metrics

            var metrics = GetMetrics(sp, log: true);

            try
            {
                Assert.AreEqual(2, (long)metrics["LionFire.Vos.Retrieve"].value!);
                Assert.AreEqual(2, (long)metrics["LionFire.Vos.Retrieve.Batch"].value!);
                Assert.AreEqual(1, (long)metrics["LionFire.Persisters.SharpZipLib.StreamRead"].value!);
                Assert.AreEqual(1, (long)metrics["LionFire.Persistence.Filesystem.Exists"].value!);
                Assert.AreEqual(1, (long)metrics["LionFire.Persistence.Filesystem.FileExistsC"].value!);
                Assert.AreEqual(1, (long)metrics["LionFire.Persistence.Filesystem.OpenReadStream"].value!);
                Assert.AreEqual(6, metrics.Count);
            }
            catch
            {
                GenerateAsserts(metrics);
                throw;
            }

            #endregion

            sp.GetRequiredService<ILogger<TestLog>>().LogInformation("/".ToVobReference().GetVob().AllMountsRecursive().DumpList("Mounts").ToString());
        });
        Serilog.Log.CloseAndFlush();
    }

    #region Common

    private void ValidateListing(IResolveResult<Metadata<IEnumerable<Listing<object>>>> resolveResult)
    {
        // TODO: Add child folder (with child file that isn't listed) to this Zip directory
        var expecteds = new HashSet<string>{
            "TestClass.json",
            "TestTargetFile.txt",
        };

        Assert.IsTrue(resolveResult.IsSuccess);
        Assert.IsTrue(resolveResult.HasValue, "No value");
        Assert.IsNotNull(resolveResult.Value);
        Assert.IsInstanceOfType<RetrieveResult<Metadata<IEnumerable<Listing<object>>>>>(resolveResult);
        var r = (RetrieveResult<Metadata<IEnumerable<Listing<object>>>>)resolveResult;
        Assert.IsTrue(r.Flags.HasFlag(PersistenceResultFlags.Found), "Not Found");
        Assert.IsFalse(r.Flags.HasFlag(PersistenceResultFlags.NotFound), "Has NotFound flag");

        Assert.AreEqual(2, resolveResult.Value.Value.Count());

        Debug.WriteLine("Listings:");
        foreach (var listing in resolveResult.Value.Value)
        {
            Debug.WriteLine($" - {listing.Name}");
            Assert.IsTrue(expecteds.Remove(listing.Name), "Unexpected listing: " + listing.Name);
        }
        Assert.AreEqual(0, expecteds.Count, "At least one expected item not found");
    }

    #endregion
}

