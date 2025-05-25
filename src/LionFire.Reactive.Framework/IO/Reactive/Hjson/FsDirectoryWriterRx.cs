namespace LionFire.IO.Reactive.Hjson;

public class FsDirectoryWriterRx<TKey, TValue>
    : DirectoryWriterRx<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    protected override IDirectoryAsync Directory => FsDirectory.Instance;

    public FsDirectoryWriterRx(DirectorySelector dir, DirectoryTypeOptions directoryTypeOptions) : base(dir, directoryTypeOptions)
    {
    }
}

