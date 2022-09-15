#nullable enable

using LionFire.Execution;
using LionFire.Referencing;
using LionFire.Vos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
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
    //public IEnumerable<IArchivePlugin> ArchivePlugins { get; }

    public SortedList<int, IArchivePlugin> ArchivePlugins { get; }
    public IEnumerable<string> ArchiveExtensions { get; }

    public ArchiveAdapter(IVob vob, ArchiveOptions options, ILogger<ArchiveAdapter> logger, IEnumerable<IArchivePlugin> archivePlugins)
    {
        Vob = vob;
        Options = options;
        Logger = logger;

        ArchivePlugins = new SortedList<int, IArchivePlugin>(archivePlugins.ToDictionary(a => a.Priority ?? a.GetType().GetHashCode()));

        ArchiveExtensions = new List<string>(ArchivePlugins.SelectMany(p => p.Value.Extensions));
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

    public  Task TryAutoMountArchives(VosPersister vosPersister, IReferencable<IVobReference> referencable)
    {
        throw new NotImplementedException();

        //bool ExtensionFilterFunc(string ext) => ArchiveExtensions.Contains(ext);

        // TODO: Instead of RetrieveAll, use List( filter ), with a filter that's either a Predicate<string filename>

        //await foreach (var archiveResultMany in vosPersister.RetrieveAll<Metadata<IEnumerable<Listing<IArchive>>>>(referencable))
        //{
        //    //    var listResult = await vosPersister.List<object>(referencable, new ListFilter
        //    //{
        //    //    Flags = ItemFlags.File,
        //    //    FilenameFilter = ExtensionFilterFunc,
        //    //});
        //    //if (listResult.IsSuccess != true)
        //    //{
        //    //    Logger.LogWarning($"TryAutoMountArchives failed to retrieve list of files(result: {listResult.IsSuccess}): " + listResult);
        //    //    return;
        //    //}

        //    foreach (var archiveResult in listResult.Value)
        //    {
        //        //    if (archiveResultMany.IsSuccess != true)
        //        //{
        //        //    Logger.LogWarning($"TryAutoMountArchives failed to retrieve item (result: {archiveResultMany.IsSuccess}): " + archiveResultMany);
        //        //    continue;
        //        //}

        //        Logger.LogCritical($"NEXT: implement FilenameFilter.  Then: found archive file {archiveResult.Name}. Next, get plugin");

        //        foreach (var archiveResult in archiveResultMany.Value.Value)
        //        {
        //            var ext = LionPath.GetExtension(archiveResult.Value);
        //            foreach (var plugin in MatchingPluginsForExtension())
        //            {
        //                Logger.LogCritical($"NEXT: found {plugin.Name} archive: {archiveResult.Value}");
        //            }
        //        }

        //    }
        //    //var archiveName = archivePlugin.GetArchiveSubpath();
        //    //            if (archiveName != null)
        //    //            {
        //    //                Trace.WriteLine($"Vob found archive: {archiveName} at {referencable.Reference.Path}");
        //    //            }
    }

    public IEnumerable<IArchivePlugin> MatchingPluginsForExtension(string extension)
    {
        foreach (var plugin in ArchivePlugins.Values)
        {
            if (plugin.Extensions.Contains(extension)) { yield return plugin; }
        }
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
                    // TODO: how to avoid infinite recursion?
                    //if (context.Flags?.Contains("ArchiveScan") != true)
                    //{
                    //    await archivePlugin.TryAutoMountArchives(vosPersister, context.Referencable);
                    //}
                }
            }
        }
    }
}




public interface IArchivePlugin
{
    int? Priority { get; }
    string Name { get; }
    IEnumerable<string> Extensions { get; }
    Task<bool> Exists(string pathToArchive, string pathInArchive);
    Task<string> ReadAllText(string pathToArchive, string pathInArchive);
    Task Write(string pathToArchive, string pathInArchive, string textContents);
}


public class ZipPlugin : IArchivePlugin
{
    public string Name => "Zip";

    public int? Priority { get; set; }

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

#if false  // 220902 brainstorm from scratch - ideas for ArchivePlugin

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
