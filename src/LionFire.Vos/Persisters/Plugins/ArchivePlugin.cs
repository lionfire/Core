#nullable enable

using LionFire.Referencing;
using LionFire.Vos;
using Microsoft.Extensions.Logging;

namespace LionFire.Persistence.Persisters.Vos;

public interface IArchive { }

public class ArchivePluginOptions
{
    public string Prefix { get; set; } = "]";
    public string Suffix { get; set; } = "[";
}

public class ArchivePlugin //: IVosPlugin
{
    public ArchivePluginOptions Options { get; }
    public ILogger<ArchivePlugin> Logger { get; }

    public ArchivePlugin(IVob vob, ArchivePluginOptions options, ILogger<ArchivePlugin> logger)
    {
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


