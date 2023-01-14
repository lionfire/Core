using System.Diagnostics;
using static ZipTest;
using static TestHostBuilder;

namespace Test_;

[TestClass]
public class _Setup
{
    [TestMethod]
    public void _() =>
        RunTest(async sp =>
        {
            var handle = ZipVobReference.GetReadHandle<byte[]>(sp);

            var exists = await handle.Exists().ConfigureAwait(false);
            Assert.IsTrue(exists);

            var resolveResult = await handle.Resolve().ConfigureAwait(false);
            Assert.IsTrue(resolveResult.IsSuccess);
            Assert.IsTrue(resolveResult.HasValue);
            Assert.IsNotNull(resolveResult.Value);
            Assert.IsTrue(resolveResult.Value.Length > 100);
            Debug.WriteLine($"{TestSourceFile} size: {resolveResult.Value.Length}");
        });
}

