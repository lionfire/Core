using LionFire.Persistence.Filesystemlike;
using Microsoft.Extensions.Logging;

namespace LionFire.IO.Reactive;

public abstract class FsDirectoryReaderRx<TKey, TValue>
        : DirectoryReaderRx<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
{
    protected FsDirectoryReaderRx(DirectorySelector dir, DirectoryTypeOptions directoryTypeOptions, ILogger logger, bool deferInit = false) : base(dir, directoryTypeOptions, logger, deferInit: deferInit)
    {
    }

    protected override IDirectoryAsync Directory => FsDirectory.Instance;
}
