using LionFire.Referencing;

namespace LionFire.IO.Reactive;


public class DirectorySelector
{
    //public DirectorySelector() { }
    public DirectorySelector(string path) { Path = path; }
    public static implicit operator DirectorySelector(string path) => new(path) { };

    public string Path { get; set; }
    public bool Recursive { get; set; }
    //public int RecursionDepth { get; set; } = 1; // FUTURE

}


public class DirectoryReferenceSelector
{
    //public DirectorySelector() { }
    public DirectoryReferenceSelector(IReference path) { Path = path; }

    public IReference Path { get; set; }
    public bool Recursive { get; set; }
    //public int RecursionDepth { get; set; } = 1; // FUTURE

}
public static class DirectoryReferenceSelectorX
{
    public static DirectoryReferenceSelector ToDirectoryReferenceSelector(this IReference path) => new DirectoryReferenceSelector(path) { };

}

