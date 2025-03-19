using LionFire.Persistence.Filesystemlike;

namespace LionFire.IO.Reactive;

public class DirectoryTypeOptions
{
    public string? SecondExtension { get; set; }
    public IFileExtensionConvention? ExtensionConvention { get; set; }
}
