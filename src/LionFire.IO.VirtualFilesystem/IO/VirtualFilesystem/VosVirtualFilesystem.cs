namespace LionFire.IO.Vos;

public class VosVirtualFilesystem : IVirtualFilesystem // MOVE
{
    public Task DeleteFile(string path)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DirectoryExists(string path)
    {
        throw new NotImplementedException();
    }

    public Task<bool> FileExists(string path)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetDirectories(string path)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetFiles(string path)
    {
        throw new NotImplementedException();
    }

    public Task<string> ReadAllText(string path, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task WriteText(string path, string? contents, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
