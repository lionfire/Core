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
using System.Diagnostics;
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
public class ExpanderMounter : IParentable<IVob>
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
    public ILogger<ExpanderMounter> Logger { get; }

    public SortedList<int, IExpanderPlugin> Expanders { get; }
    public IEnumerable<string> ArchiveExtensions { get; }

    #region Lifecycle

    public ExpanderMounter(IVob vob, ExpansionMountOptions options, ILogger<ExpanderMounter> logger, IEnumerable<IExpanderPlugin> archivePlugins)
    {
        Parent = vob;
        Options = options;
        Logger = logger;

        Expanders = new SortedList<int, IExpanderPlugin>(archivePlugins.ToDictionary(a => a.Priority ?? a.GetType().GetHashCode()));

        ArchiveExtensions = new List<string>(Expanders.SelectMany(p => p.Value.Extensions));
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

    public async Task<bool> Scan(BeforeReadEventArgs args)
    {
        bool foundExpanderMounterVob = false;

        foreach (var x in args.Vob.GetAcquireEnumerator2<IVob, ExpanderScanState>(includeNull: true).TakeWhile(z =>
        {
            var shouldContinue = !foundExpanderMounterVob;
            if (z.Item2 == this.Vob) foundExpanderMounterVob = true;
            return shouldContinue;
        }))
        {
            var state = x.Item1;
            if (state == null)
            {
                state = x.Item2.GetOrAddOwn(_ => new ExpanderScanState());
            }

            bool shouldScan =
                !state.LastScan.HasValue
                || Options.TimeBetweenScans.HasValue && DateTime.UtcNow - state.LastScan > Options.TimeBetweenScans.Value
                ;

            if (shouldScan)
            {
                state.LastScan = DateTime.UtcNow;
                await TryAutoMountArchives(args.Persister, x.Item2);
            }
        }

        return true; // TODO - opt-in to caching logic, and file watching to avoid excess scans
    }

    private static AsyncLocal<bool> IsInArchiveScan = new();


    public async Task TryAutoMountArchives(VosPersister vosPersister, IVob vob)
    {
        IReferencable<IVobReference> referencable = vob.Reference;
        ExpansionMountStates? states = null;

        if (IsInArchiveScan.Value) { return; }
        else { IsInArchiveScan.Value = true; }
        try
        {
            bool ExtensionFilterFunc(string ext) => ArchiveExtensions.Contains(ext);

            //TODO: Instead of RetrieveAll, use List(filter ), with a filter that's either a Predicate<string filename>

            await foreach (var archiveResultsBatch in vosPersister.RetrieveBatches<Metadata<IEnumerable<Listing<IArchive>>>>(vob))
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
                    if (archiveResultsBatch.HasValue && archiveResultsBatch.Value.Value != null)
                        foreach (var archiveResult in archiveResultsBatch.Value.Value)
                        {
                            Debug.Assert(archiveResult?.Name != null);
                            var expansionArgs = GetExpansionSubpath(archiveResult.Name);
                            if (expansionArgs.Direction == IODirection.Unspecified) continue;
                            var ext = Path.GetExtension(archiveResult.Name);
                            //var ext = LionPath.GetExtension(archiveResult.Name);
                            foreach (var plugin in MatchingExpandersForExtension(ext))
                            {
                                //var vob = vosPersister.Root[vob.Reference];
                                states ??= vob.GetOrAddOwn<ExpansionMountStates>();
                                Debug.Assert(states.States != null);
                                var state = states.States.GetOrAdd(archiveResult.Name, (n) => new ExpansionMountState { Name = n });

                                if (state.Mount == null)
                                {
                                    var expansionReference = new ExpansionReference(("/" + LionPath.Combine(vob.Reference.PathChunks, archiveResult.Name)).ToVobReference().ToString(), "");
                                    var vobMountPoint = vob;
                                    if (!string.IsNullOrEmpty(expansionArgs.Subpath))
                                    {
                                        vobMountPoint = vobMountPoint[expansionArgs.Subpath];
                                    }

                                    var upstreamMount = (archiveResultsBatch as VosPersistenceResult)?.ResolvedViaMount;
                                    var mount = vobMountPoint.Mount(expansionReference, new VobMountOptions
                                    {
                                        IsReadOnly = expansionArgs.Direction == IODirection.Read,
                                        IsWritable = expansionArgs.Direction.IsWritable(),
                                        Name = $"Expansion: {expansionReference.ToString()}",
                                        UpstreamMount = upstreamMount,
                                        //ReadPriority = -10,
                                        //WritePriority = ,
                                    });
                                    Logger.LogInformation($"ExpanderMounter mounted {archiveResult.Name}: {mount}.");
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


    public IEnumerable<IExpanderPlugin> MatchingExpandersForExtension(string extension)
    {
        // ENH: Sort by plugin priority

        extension = extension.TrimStart('.');
        foreach (var plugin in Expanders.Values)
        {
            if (plugin.Extensions.Contains(extension)) { yield return plugin; }
        }
    }

    public async Task BeforeListHandler(BeforeListEventArgs args)
    {
        if (args.ListingType != typeof(IArchive))
        {
            // TODO: how to avoid infinite recursion?
            if (args.Flags?.Contains("ArchiveScan") != true)
            {
                await Scan(args).ConfigureAwait(false);
            }
        }
    }
    public Task BeforeRetrieveHandler(BeforeRetrieveEventArgs args)
    {
        return Scan(args);
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

public interface IExpanderPlugin
{
    int? Priority { get; }
    string Name { get; }
    IEnumerable<string> Extensions { get; }
    Task<bool> Exists(string pathToArchive, string pathInArchive);
    Task<string> ReadAllText(string pathToArchive, string pathInArchive);
    Task Write(string pathToArchive, string pathInArchive, string textContents);
}

public class ZipPlugin : IExpanderPlugin
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

public class ExpanderScanState
{
    public DateTime? LastScan { get; set; }

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
