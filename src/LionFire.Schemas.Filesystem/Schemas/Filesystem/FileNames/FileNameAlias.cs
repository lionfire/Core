namespace LionFire.Schemas.Filesystem;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
public class FileNameAlias : Attribute
{
    public FileNameAlias(string alias)
    {
        Alias = alias;
    }
    public string Alias { get; set; }
}
