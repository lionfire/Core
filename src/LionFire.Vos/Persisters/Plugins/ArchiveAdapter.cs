#nullable enable

using LionFire.Referencing;
using LionFire.Vos;
using Microsoft.Extensions.Logging;
using static LionFire.Persistence.Persisters.Vos.VosPersister;

namespace LionFire.Persistence.Persisters.Vos;

public interface IArchive { }

public class ArchiveOptions
{
    public string Prefix { get; set; } = "]";
    public string Suffix { get; set; } = "[";
}

public class ArchiveAdapter //: IVosPlugin
{
    public IVob Vob { get; }
    public ArchiveOptions Options { get; }
    public ILogger<ArchiveAdapter> Logger { get; }

    public ArchiveAdapter(IVob vob, ArchiveOptions options, ILogger<ArchiveAdapter> logger)
    {
        Vob = vob;
        Options = options;
        Logger = logger;
    }

    public string? GetArchiveSubpath(string name)
    {
        if (!name.StartsWith(Options.Prefix)) { return null; }
        int indexOfEnd = name.IndexOf(Options.Suffix);
        if (indexOfEnd == -1) { return null; }

        return name[Options.Prefix.Length..indexOfEnd];
    }

    public bool ShouldScan(IVobReference reference)
    {
        return true; // TODO - opt-in to caching logic, and file watching to avoid excess scans
    }

    public async Task TryAutoMountArchives(VosPersister vosPersister, IReferencable<IVobReference> referencable)
    {
        await foreach (var archiveResult in vosPersister.RetrieveAll<Metadata<IEnumerable<Listing<IArchive>>>>(referencable))
        {
            if (archiveResult.IsSuccess != true)
            {
                Logger.LogWarning($"TryAutoMountArchives failed to retrieve item (result: {archiveResult.IsSuccess}): " + archiveResult);
                continue;
            }

            Logger.LogCritical($"NEXT: found archive: {archiveResult.Value}");

        }
        //var archiveName = archivePlugin.GetArchiveSubpath();
        //            if (archiveName != null)
        //            {
        //                Trace.WriteLine($"Vob found archive: {archiveName} at {referencable.Reference.Path}");
        //            }
    }

    
}

public class ArchiveAdapterStatic
{
    internal static async Task VosPersister_OnBeforeRetrieve(VosPersister vosPersister, RetrieveContext context)
    {
        var archivePlugin = context.Vob.TryGetNextVobNode<ArchiveAdapter>()?.Value;

        if (archivePlugin != null && context.ListingType != null)
        {
            if (context.ListingType != typeof(IArchive))
            {
                if (archivePlugin.ShouldScan(context.Referencable.Reference))
                {
                    await archivePlugin.TryAutoMountArchives(vosPersister, context.Referencable);
                }
            }
        }
    }
}


#if false  // 220902 brainstorm from scratch - ideas for ArchivePlugin


public interface IArchivePlugin
{
    IEnumerable<string> Extensions { get; }
    Task<bool> Exists(string pathToArchive, string pathInArchive);
    Task<string> ReadAllText(string pathToArchive, string pathInArchive);
    Task Write(string pathToArchive, string pathInArchive, string textContents);
}

public class ZipArchiveProvider : IArchivePlugin
{
    public IEnumerable<string> Extensions { get { yield return "zip"; } }

    public Task<bool> Exists(string pathToArchive, string pathInArchive)
    {
        throw new NotImplementedException();
    }

    public Task<string> ReadAllText(string pathToArchive, string pathInArchive)
    {
        throw new NotImplementedException();
    }
    public Task Write(string pathToArchive, string pathInArchive, string textContents)
    {
        throw new NotImplementedException();
    }
}

public class ArchiveFilesystem : IFilesystemPlugin
{
    public IEnumerable<IArchivePlugin> ArchivePlugins { get; }

    public ArchiveFilesystem(IEnumerable<IArchivePlugin> archivePlugins)
    {
        ArchivePlugins = archivePlugins;
    }

#region IFilesystem

    /// <summary>
    /// Discovering virtual directories that exist because of archives
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public IEnumerable<string> GetDirectories(string path)
    {
        throw new NotImplementedException();
    }

#endregion

#region 


#endregion

#region Archives populating a particular directory

    public IEnumerable<string> PossibleArchiveLocationsForDirectory(string directory)
    {

    }

    public IEnumerable<string> ArchivesForDirectory(string directory)
    {

    }

#endregion

    public bool IsArchive(string path)
    {

    }
}

public interface IFilesystemPlugin
{

}

#endif
