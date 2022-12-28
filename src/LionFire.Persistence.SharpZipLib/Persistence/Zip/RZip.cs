using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolves;
using MorseCode.ITask;
using ICSharpCode.SharpZipLib.Zip;
using LionFire.Ontology;
using LionFire.Persistence;
using System.Diagnostics;
using LionFire.ExtensionMethods;

namespace LionFire.Persisters.SharpZipLib_;

public class RZip : ReadHandle<IReference<ZipFile>, ZipFile>, IHas<IServiceProvider>
{
    #region Dependencies

    public IServiceProvider ServiceProvider { get; }
    IServiceProvider IHas<IServiceProvider>.Object => ServiceProvider;

    #endregion

    #region Lifecycle

    public RZip(IReference<ZipFile> reference, IServiceProvider serviceProvider) : base(reference)
    {
        ServiceProvider = serviceProvider;
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

    #endregion

    #region State

    MemoryStream? ms;
    public ICSharpCode.SharpZipLib.Zip.ZipFile? ZipFile { get; private set; }

    #endregion

    #region Resolve Reference to ZipFile (via byte[] or Stream)

    protected override async ITask<IResolveResult<ZipFile>> ResolveImpl()
    {
#if ENH
        // ENH: Try getting a Stream, unless user opted to use byte[].  Maybe use a different class of handle: RZipStream
        //var result = await Reference.Resolve<IReference, Stream>().ConfigureAwait(false);
        var streamReadHandle = this.Key.GetReadHandle<Stream>();
        var streamRetrieveResult = await streamReadHandle.Resolve().ConfigureAwait(false);

        if (streamRetrieveResult.IsSuccess() == true)
        {
            Debug.WriteLine($"Retrieved stream");
        }
#endif

        var bytesReadHandle = this.Key.GetReadHandle<byte[]>();
        var bytesRetrieveResult = await bytesReadHandle.Resolve().ConfigureAwait(false);

        if (bytesRetrieveResult.IsSuccess() != true)
        {
            return ResolveResultNotResolved<ZipFile>.Instance;
        }
        Log.Get<RZip>().Debug($"RZip Retrieved {bytesRetrieveResult.Value.Length} bytes");

        var oldMemoryStream = ms;
        if (oldMemoryStream != null)
        {
            await oldMemoryStream.DisposeAsync();
        }

        ms = new MemoryStream(bytesRetrieveResult.Value);
        ProtectedValue = new ICSharpCode.SharpZipLib.Zip.ZipFile(ms, true);
        return new ResolveResultSuccess<ZipFile>(Value);
    }

    #endregion

}
