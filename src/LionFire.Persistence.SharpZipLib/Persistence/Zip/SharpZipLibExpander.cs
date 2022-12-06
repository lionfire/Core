using LionFire.Persistence.Persisters;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Referencing;
using LionFire.Structures;
using LionFire.Vos.Mounts;
using Microsoft.Extensions.Options;
using LionFire.Persisters.Expanders;
using LionFire.Persistence;
using ICSharpCode.SharpZipLib.Zip;
using System.Net.Http.Headers;

namespace LionFire.Persisters.SharpZipLib_;

public class SharpZipLibExpander : ExpanderPersister, ISupportsFileExtensions
{
    //ICSharpCode.SharpZipLib.Zip.
    public IOptionsMonitor<SharpZipLibExpanderOptions> OptionsMonitor { get; }


    public SharpZipLibExpander(IOptionsMonitor<SharpZipLibExpanderOptions> optionsMonitor)
    {
        OptionsMonitor = optionsMonitor;
    }

    public List<string> FileExtensions => OptionsMonitor.CurrentValue.FileExtensions;


    public override Task<RetrieveResult<TValue>> Retrieve<TValue>(ExpanderReadHandle<TValue> readHandle)
    {
        throw new NotImplementedException();
    }

    public override Type? SourceReadType() => typeof(byte[]); // TODO: also support Stream?

    public override Type? SourceReadTypeForReference(IReference reference)
    {
        throw new NotImplementedException();
    }

    public override IReadHandle? TryGetReadHandle(IReference sourceReference)
    {
        var ext = Path.GetExtension(sourceReference.Path)?.TrimStart('.');

        switch (ext)
        {
            case "zip":
                return new RZip(sourceReference.Cast<ZipFile>());
            //case "tar":
            //    break;
            //case "bz2":
            //    break;
            //case "bzip2":
            //    break;
            //case "gzip":
            //    break;
            default:
                throw new ArgumentException("unknown/unimplemented extension and autodetect type is not yet implemented");
        }
    }
}
