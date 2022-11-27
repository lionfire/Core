using LionFire.Persistence.Persisters;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Structures;
using LionFire.Vos.Mounts;
using Microsoft.Extensions.Options;

namespace LionFire.Persistence.Persisters.SharpZipLib_;

//public enum PersisterCapabilities
//{
//    Unspecified = 0,
//    Read = 1 << 0,
//    Write = 1 << 1,
//    ByteArray = 1 << 2,

//}

public class ArchivePersisterOptions : IMultiTypePersisterProviderOptions, INamedPersisterOptions
{
    public Type? PersisterType { get; set; }

    /// <summary>
    /// If null, defaults to the name specified to Microsoft.Extensions.Options when configuring
    /// </summary>
    public string? PersisterName { get; set; }
}

public class SharpZipLibPersister : PersisterBase<ArchivePersisterOptions>
{
    //ICSharpCode.SharpZipLib.Zip.

    public SharpZipLibPersister(OptionsName optionsName, IOptionsMonitor<ArchivePersisterOptions> optionsMonitor)
    {
        //optionsMonitor.CurrentValue;
    }
}

public class SharpZipLibArchive
{
}

public interface IMountOptions
{

}

public class ArchiveMountOptions : IMountOptions
{
    //public PersisterCapabilities PersisterCapabilities = PersisterCapabilities.Read | PersisterCapabilities.Write | PersisterCapabilities.ByteArray;

}
