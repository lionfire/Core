namespace LionFire.IO;

public abstract class FilesystemBase
{
    public abstract IEnumerable<string> GetDirectories(string path);

    public virtual IEnumerable<string> GetDirectories(string path, string searchPattern) => GetDirectories(path).Where(d => MatchesPattern(d, searchPattern));

    protected virtual bool MatchesPattern(string path, string pattern)
    {
        throw new NotImplementedException();
    }
}
