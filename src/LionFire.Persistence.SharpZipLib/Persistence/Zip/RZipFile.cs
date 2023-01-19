using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Resolves;
using MorseCode.ITask;
using ICSharpCode.SharpZipLib.Zip;
using LionFire.Ontology;
using LionFire.Persistence;
using System.Diagnostics;
using LionFire.ExtensionMethods;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System.Diagnostics.Metrics;
using System.IO;

namespace LionFire.Persisters.SharpZipLib_;

// RENAME to RZipFile
public class RZipFile : ReadHandle<IReference<ZipFile>, ZipFile>
    //, IHas<IServiceProvider> 
{
    #region Dependencies

    //public IServiceProvider ServiceProvider { get; }
    //IServiceProvider IHas<IServiceProvider>.Object => ServiceProvider;

    #endregion

    #region Lifecycle

    public RZipFile(IReference reference) : base(reference.Cast<ZipFile>())
    {
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

    IReadHandle<Stream>? streamReadHandle;
    ILazyResolveResult<Stream>? streamRetrieveResult;

    public override void DiscardValue()
    {
        lock (_lock)
        {
            base.DiscardValue();
            // TODO: Should TValue be changed to also include these?
            streamReadHandle?.DiscardValue();
            streamRetrieveResult = null;
            streamRetrieveResult?.Value?.Dispose();
            streamReadHandle = null;
        }
    }
    private object _lock = new();

    private static readonly Meter Meter = new("LionFire.Persisters.SharpZipLib", "1.0");
    private static readonly Counter<long> StreamReadC = Meter.CreateCounter<long>("StreamRead");
    private static readonly Counter<long> StreamReadBytesC = Meter.CreateCounter<long>("StreamReadBytes");

    protected override async ITask<IResolveResult<ZipFile>> ResolveImpl()
    {
        //#if ENH
        // ENH: Try getting a Stream, unless user opted to use byte[].  Maybe use a different class of handle: RZipStream
        //var result = await Reference.Resolve<IReference, Stream>().ConfigureAwait(false);

        bool noop = streamRetrieveResult?.HasValue == true;

        // TODO TOTHREADSAFETY
        if (!noop)
        {
            StreamReadC.IncrementWithContext();
            streamRetrieveResult ??= await (streamReadHandle ??= this.Key.GetReadHandle<Stream>())
                .TryGetValue().ConfigureAwait(false);
        }

        if (streamRetrieveResult.IsSuccess() == true)
        {
            if (!noop) { StreamReadBytesC.IncrementWithContext(streamRetrieveResult!.Value.Length); }
            Logger.Debug($"{(noop ? "[NOOP] " : "")} RZipFile Retrieved stream of length {streamRetrieveResult!.Value.Length} bytes from {Key} ");
            ProtectedValue = new ICSharpCode.SharpZipLib.Zip.ZipFile(streamRetrieveResult.Value, true);
            return new ResolveResultSuccess<ZipFile>(Value);
        }
        else
        //#endif
        {
            var bytesReadHandle = this.Key.GetReadHandle<byte[]>();
            var bytesRetrieveResult = await bytesReadHandle.Resolve().ConfigureAwait(false);

            if (bytesRetrieveResult.IsSuccess() != true)
            {
                return ResolveResultNotResolved<ZipFile>.Instance;
            }
            Logger.Debug($"RZipFile Retrieved byte array of {bytesRetrieveResult.Value?.Length} bytes");

            var oldMemoryStream = ms;
            if (oldMemoryStream != null)
            {
                await oldMemoryStream.DisposeAsync();
            }

            if (bytesRetrieveResult.Value == null)
            {
                ProtectedValue = null;
                return ResolveResultNotResolved<ZipFile>.Instance;
            }
            else
            {
                ms = new MemoryStream(bytesRetrieveResult.Value);
                ProtectedValue = new ICSharpCode.SharpZipLib.Zip.ZipFile(ms, false);
                return new ResolveResultSuccess<ZipFile>(Value);
            }
        }
    }

    #endregion

    static ILogger Logger => Log.Get<RZipFile>();
}
