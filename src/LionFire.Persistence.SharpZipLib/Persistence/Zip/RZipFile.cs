using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Data.Gets;
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
using Timer = System.Timers.Timer;

namespace LionFire.Persisters.SharpZipLib_;

public class DelayCloser
{

}

public class ZipFileOptions
{
    public bool LeaveOpen { get; set; } = false;

    public static ZipFileOptions Default { get; set; } = new();
}

public class RZipFile : ReadHandle<IReference<ZipFile>, ZipFile>
{
    //public IServiceProvider ServiceProvider { get; }

    #region Lifecycle

    public RZipFile(IReference reference) : base(reference.Cast<ZipFile>())
    {
        //ServiceProvider = serviceProvider;
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

    #endregion

    #region Resolve Reference to ZipFile (via byte[] or Stream)

    IReadHandle<Stream>? streamReadHandle;
    IGetResult<Stream>? streamRetrieveResult;

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


    Timer DelayCloseTimer;
    private void DelayClose()
    {
        if (DelayCloseTimer == null)
        {
            DelayCloseTimer = new Timer(300);
            DelayCloseTimer.Elapsed += DelayCloseTimer_Elapsed;
            DelayCloseTimer.AutoReset = false;
        }
        else
        {
            DelayCloseTimer.Stop();
        }
        DelayCloseTimer.Start();
    }

    private void DelayCloseTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        Logger.LogInformation("DelayCloseTimer_Elapsed, discarding: ", Reference);
        DiscardValue();
    }

    protected override async ITask<IGetResult<ZipFile>> GetImpl(CancellationToken cancellationToken = default)
    {
        ResolveResultSuccess<ZipFile> onSuccess(ZipFile value)
        {
            //DelayClose();
            return new ResolveResultSuccess<ZipFile>(Value);
        }
        //#if ENH
        // ENH: Try getting a Stream, unless user opted to use byte[].  Maybe use a different class of handle: RZipStream
        //var result = await Reference.Resolve<IReference, Stream>().ConfigureAwait(false);

        bool noop = streamRetrieveResult?.HasValue == true;

        // TODO TOTHREADSAFETY
        if (!noop)
        {
            StreamReadC.IncrementWithContext();
            streamRetrieveResult ??= await (streamReadHandle ??= this.Key.GetReadHandle<Stream>())
                .GetIfNeeded().ConfigureAwait(false);
        }

        if (streamRetrieveResult.IsSuccess() == true)
        {
            if (!noop) { StreamReadBytesC.IncrementWithContext(streamRetrieveResult!.Value.Length); }
            Logger.Debug($"{(noop ? "[NOOP] " : "")} RZipFile Retrieved stream of length {streamRetrieveResult!.Value.Length} bytes from {Key} ");
            if (ReadCacheValue != null)
            {
                Logger.Warn($"{nameof(ReadCacheValue)} != null");
            }
            //else
            //{
            streamRetrieveResult.Value.Seek(0, SeekOrigin.Begin);
            ReadCacheValue = new ICSharpCode.SharpZipLib.Zip.ZipFile(streamRetrieveResult.Value, leaveOpen: ZipFileOptions.Default.LeaveOpen);
            //}
            return onSuccess(Value);
        }
        else
        //#endif
        {
            var bytesReadHandle = this.Key.GetReadHandle<byte[]>();
            var bytesRetrieveResult = await bytesReadHandle.Get().ConfigureAwait(false);

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
                ReadCacheValue = default;
                return ResolveResultNotResolved<ZipFile>.Instance;
            }
            else
            {
                ms = new MemoryStream(bytesRetrieveResult.Value);
                ReadCacheValue = new ICSharpCode.SharpZipLib.Zip.ZipFile(ms, false);
                return onSuccess(Value);
            }
        }
    }

    #endregion

    static ILogger Logger => Log.Get<RZipFile>();

}
