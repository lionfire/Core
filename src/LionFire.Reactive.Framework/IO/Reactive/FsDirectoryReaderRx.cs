using LionFire.Persistence.Filesystemlike;

namespace LionFire.IO.Reactive;

public abstract class FsDirectoryReaderRx<TKey, TValue>
        : DirectoryReaderRx<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
{
    protected FsDirectoryReaderRx(DirectorySelector dir, DirectoryTypeOptions directoryTypeOptions) : base(dir, directoryTypeOptions)
    {
    }

    protected override IDirectoryAsync Directory => FsDirectory.Instance;
}
