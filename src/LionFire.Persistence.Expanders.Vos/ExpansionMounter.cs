#nullable enable

using LionFire;
using LionFire.Execution;
using LionFire.ExtensionMethods;
using LionFire.ExtensionMethods.Acquisition;
using LionFire.IO;
using LionFire.Ontology;
using LionFire.Persistence;
using LionFire.Persistence.Persisters.Vos;
using LionFire.Referencing;
using LionFire.Vos;
using LionFire.Vos.Mounts;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using static LionFire.Persistence.Persisters.Vos.VosPersister;
using static LionFire.Reflection.GetMethodEx;

namespace LionFire.Persisters.Expanders;

public interface IArchive { }


///// <summary>
///// 
///// </summary>
///// <remarks>If needs to be detached, also implement IDisposable</remarks>
///// <typeparam name="T"></typeparam>
//public interface IAttacher<T>
//{
//    void Attach(T vosPersister);
//}

// Hooks:
//  - BeforeList: 
public class ExpansionMounter : IParentable<IVob>
{

    #region Relationships

    public IVob? Parent { get; protected set; }
    IVob? IParentable<IVob>.Parent { get => Parent; set => Parent = value; }
    IVob? Vob => Parent;

    #region Derived

    public VosPersister? VosPersister => Parent?.Root.Acquire<VosPersister>();

    //public VosListEvents VosListEvents => Parent?.AcquireOwn<VosPersister>();

    #endregion

    #endregion

    public ExpansionMountOptions Options { get; }
    public ILogger<ExpansionMounter> Logger { get; }
    //public IEnumerable<IArchivePlugin> ArchivePlugins { get; }

    public SortedList<int, IArchivePlugin> ArchivePlugins { get; }
    public IEnumerable<string> ArchiveExtensions { get; }

    #region Lifecycle

    public ExpansionMounter(IVob vob, ExpansionMountOptions options, ILogger<ExpansionMounter> logger, IEnumerable<IArchivePlugin> archivePlugins)
    {
        Parent = vob;
        Options = options;
        Logger = logger;

        ArchivePlugins = new SortedList<int, IArchivePlugin>(archivePlugins.ToDictionary(a => a.Priority ?? a.GetType().GetHashCode()));

        ArchiveExtensions = new List<string>(ArchivePlugins.SelectMany(p => p.Value.Extensions));
    }

    #endregion

    //#region IAttacher

    //public void Attach(VosPersister persister)
    //{
    //    persister.BeforeRetrieve.Add(1, c => ArchiveAdapterStatic.VosPersister_OnBeforeRetrieve(c));
    //}
    //public void Detach(VosPersister persister)
    //{
    //    persister.BeforeRetrieve.Remove(1);
    //}

    //#endregion

    public (string? Subpath, IODirection Direction) GetExpansionSubpath(string name)
    {
        (string?, IODirection) NotFound() => (null, IODirection.Unspecified);

        int index = 0;
        if (name.StartsWith('.')) { index++; }

        char prefixChar = Options.PrefixCharacter;
        char suffixChar = Options.SuffixCharacter;
        int prefixCount = 0;
        for (; name[index] == prefixChar; prefixCount++, index++) { }

        if (prefixCount == 0) return NotFound();

        int nameIndex = index;

        int suffixStart;
        int suffixCount = 0;

        for (suffixStart = index = name.IndexOf(suffixChar, index)
            ; suffixStart > 0 && index < name.Length
            ; suffixStart = index = name.IndexOf(suffixChar, index))
        {
            suffixCount = 0;
            for (; name[index] == suffixChar && suffixCount < prefixCount; suffixCount++, index++) { }

            if (suffixCount == prefixCount) { break; }
        }

        if (suffixCount != prefixCount) return NotFound();


        //int indexOfEnd = name.IndexOf(Options.Suffix);
        //if (indexOfEnd == -1) { return null; }

        return (name[nameIndex..suffixStart], IODirectionFromCharCount(prefixCount));

        IODirection IODirectionFromCharCount(int count)
        => count switch
        {
            1 => IODirection.Read,
            2 => IODirection.ReadWrite,
            3 => IODirection.Write,
            _ => IODirection.Unspecified,
        };
    }

    public bool ShouldScan(IVobReference reference)
    {
        return true; // TODO - opt-in to caching logic, and file watching to avoid excess scans
    }

    private static AsyncLocal<bool> IsInArchiveScan = new();


    public async Task TryAutoMountArchives(VosPersister vosPersister, IReferencable<IVobReference> referencable)
    {
        ExpansionMountStates? states = null;

        if (IsInArchiveScan.Value) { return; }
        else { IsInArchiveScan.Value = true; }
        try
        {
            bool ExtensionFilterFunc(string ext) => ArchiveExtensions.Contains(ext);

            //TODO: Instead of RetrieveAll, use List(filter ), with a filter that's either a Predicate<string filename>

            await foreach (var archiveResultsBatch in vosPersister.RetrieveBatches<Metadata<IEnumerable<Listing<IArchive>>>>(referencable))
            {
                //var listResult = await vosPersister.List<object>(referencable, new ListFilter
                //{
                //    Flags = ItemFlags.File,
                //    FilenameFilter = ExtensionFilterFunc,
                //});
                //if (listResult.IsSuccess != true)
                //{
                //    Logger.LogWarning($"TryAutoMountArchives failed to retrieve list of files(result: {listResult.IsSuccess}): " + listResult);
                //    return;
                //}

                if (archiveResultsBatch.Error != null)
                {
                    Logger.LogWarning($"TryAutoMountArchives error when retrieving item (result: {archiveResultsBatch.IsSuccess}): " + archiveResultsBatch);
                    continue;
                }

                if (archiveResultsBatch.IsSuccess == true)
                {
                    foreach (var archiveResult in archiveResultsBatch.Value.Value)
                    {
                        var archiveName = GetExpansionSubpath(archiveResult.Name);

                        if (archiveName.Direction != IODirection.Unspecified)
                        {
                            var ext = Path.GetExtension(archiveResult.Name);
                            //    var ext = LionPath.GetExtension(archiveResult.Value);
                            foreach (var plugin in MatchingPluginsForExtension(ext))
                            {
                                var vob = vosPersister.Root[referencable.Reference];
                                states ??= vob.Acquire<ExpansionMountStates>();

                                var state = states.States.GetOrAdd(archiveResult.Name, (n) => new ExpansionMountState { Name = n });

                                if (state.Mount == null)
                                {
                                    var expansionReference = new VobReference(LionPath.Combine(referencable.Reference.PathChunks, archiveResult.Name))
                                    {
                                        Persister = PersisterName,
                                    };
                                    vob.Mount(expansionReference, new VobMountOptions
                                    {
                                        IsReadOnly = archiveName.Direction == IODirection.Read,
                                        IsWritable = archiveName.Direction.IsWritable(),
                                        Name = $"Expansion: {expansionReference.Path}",
                                        //ReadPriority = ,
                                        //WritePriority = ,
                                    });
                                }

                                Logger.LogCritical($"NEXT: implement FilenameFilter.  Then: found archive file {archiveResult.Name}. Next, get plugin");
                                //        Logger.LogCritical($"NEXT: found {plugin.Name} archive: {archiveResult.Value}");
                                //                Trace.WriteLine($"Vob found archive: {archiveName} at {referencable.Reference.Path}");
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            IsInArchiveScan.Value = false;
        }
    }

    public const string PersisterName = "expand";


    public IEnumerable<IArchivePlugin> MatchingPluginsForExtension(string extension)
    {
        extension = extension.TrimStart('.');
        foreach (var plugin in ArchivePlugins.Values)
        {
            if (plugin.Extensions.Contains(extension)) { yield return plugin; }
        }
    }

    public async Task BeforeListHandler(BeforeListEventArgs args)
    {
        if (args.ListingType != typeof(IArchive))
        {
            var reference = args.Referencable.Reference;
            if (ShouldScan(reference))
            {
                // TODO: how to avoid infinite recursion?
                if (args.Flags?.Contains("ArchiveScan") != true)
                {
                    await TryAutoMountArchives((VosPersister)args.Persister, reference);
                }
            }
        }
    }
}


//public class VosRetrieveContext : RetrieveContext<IVobReference>
//{
//    public IVob Vob { get; set; }
//}

//public class ArchiveAdapterStatic // OLD
//{
//    internal static async Task VosPersister_OnBeforeRetrieve(VosRetrieveContext context)
//    {

//        var archivePlugin = context.Vob.TryGetNextVobNode<ArchiveAdapter>()?.Value;

//        if (archivePlugin != null && context.ListingType != null)
//        {
//            if (context.ListingType != typeof(IArchive))
//            {
//                if (archivePlugin.ShouldScan(context.Reference))
//                {
//                    // TODO: how to avoid infinite recursion?
//                    if (context.Flags?.Contains("ArchiveScan") != true)
//                    {
//                        await archivePlugin.TryAutoMountArchives((VosPersister)context.Persister, context.Reference);
//                    }
//                }
//            }
//        }
//    }
//}

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
public class ExpansionMountState
{
    public string Name { get; set; }
    public Mount Mount { get; set; }
}

public class ExpansionMountStates
{
    public ConcurrentDictionary<string, ExpansionMountState> States { get; } = new();
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
