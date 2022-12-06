using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolves;
using MorseCode.ITask;
using LionFire.ExtensionMethods.Poco.Resolvables;
using ICSharpCode.SharpZipLib.Zip;
using LionFire.Ontology;

namespace LionFire.Persisters.SharpZipLib_;

public class RZip : ReadHandle<IReference<ZipFile>, ZipFile>, IHas<IServiceProvider>
{
    public ICSharpCode.SharpZipLib.Zip.ZipFile? ZipFile { get; private set; }
    public IServiceProvider ServiceProvider { get; }
    IServiceProvider IHas<IServiceProvider>.Object => ServiceProvider;


    MemoryStream? ms;

    public RZip(IReference<ZipFile> reference, IServiceProvider serviceProvider) : base(reference)
    {
        ServiceProvider = serviceProvider;
    }

    protected override async ITask<IResolveResult<ZipFile>> ResolveImpl()
    {
        //#error NEXT: Test with a valid path, and figure out if I'm set up to retrieve byte[] (or Stream) from a vos file

        // ENH: Try getting a Stream, unless user opted to use byte[].  Maybe use a different class of handle: RZipStream
        //var result = await Reference.Resolve<IReference, Stream>().ConfigureAwait(false);

        var result = await Reference.Resolve<IReference, byte[]>().ConfigureAwait(false);

        if (result.IsSuccess != true)
        {
            return ResolveResultNotResolved<ZipFile>.Instance;
        }

        var oldMemoryStream = ms;
        if (oldMemoryStream != null)
        {
            await oldMemoryStream.DisposeAsync();
        }

        ms = new MemoryStream(result.Value);
        ProtectedValue = new ICSharpCode.SharpZipLib.Zip.ZipFile(ms, true);
        return new ResolveResultSuccess<ZipFile>(Value);
    }
    public override void Dispose()
    {
        var oldMemoryStream = ms;
        ms = null;
        if (oldMemoryStream != null)
        {
            oldMemoryStream.Dispose();
        }

        base.Dispose();
    }
}

//public class SharpZipLibReadHandle : ReadHandleBase<IReference<IReadHandle<byte[]>>, IReadHandle<byte[]>>, IValidatingReadHandle
//{

//    protected override ITask<IResolveResult<byte[]>> ResolveImpl()
//    {
//        throw new NotImplementedException();
//    }

//    public Task<(bool IsValid, string? FailReason)> IsValid(ValidityCheckDetail validityCheckDetail)
//    {
//        throw new NotImplementedException();
//    }
//}
