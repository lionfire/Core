using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Persisters.Expanders;
using LionFire.Vos;
using LionFire.Hosting;

namespace ExpandMount_;

[TestClass]
public class _Resolve
{
    public static IHostBuilder H => TestHostBuilder.ExpandMount;

    public static readonly string VobReferencePath = $"/test/ExpandMount/TestTargetDir/TestClass.json";

    [TestMethod]
    public void _()
    {
        H.Run(async sp =>
        {
            var HandleProvider = sp.GetRequiredService<IReadHandleProvider<IVobReference>>();

            var reference = VobReferencePath.ToVobReference<TestClass>();
            var handle = HandleProvider.GetReadHandle(reference);
            Assert.AreEqual(handle.Reference.Path, reference.Path);
            //Assert.IsTrue(ReferenceEquals(handle.Reference, reference));
            //Assert.AreSame(handle.Reference, reference);

            var exists = await handle.Exists().ConfigureAwait(false);
            Assert.IsTrue(exists);

            var resolveResult = await handle.Resolve().ConfigureAwait(false);
            Assert.IsTrue(resolveResult.IsSuccess);
            Assert.IsTrue(resolveResult.HasValue);
            Assert.IsNotNull(resolveResult.Value);
            Assert.AreEqual("Test Name", resolveResult.Value.Name);
            Assert.AreEqual(123, resolveResult.Value.Number);
        });
    }
}
