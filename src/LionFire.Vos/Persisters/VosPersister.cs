using LionFire.Cqrs;
using LionFire.IO;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Vos;
using LionFire.Vos.Mounts;
using LionFire.Vos.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LionFire.Persistence.Persisters.Vos;

public class VosPersister :
    SerializingPersisterBase<VosPersisterOptions>
    , IPersister<IVobReference>
//, IFilesystemPersistence<TReference, TPersistenceOptions>
//, IWriter<string> // FUTURE?
//, IReader<string> // FUTURE?
{

    //#region RootName

    //[SetOnce]
    //public string RootName
    //{
    //    get => rootName;
    //    set
    //    {
    //        if (rootName == value) return;
    //        if (rootName != default) throw new AlreadySetException();
    //        rootName = value;
    //    }
    //}
    //private string rootName;

    //#endregion

    #region Root

    public IRootVob Root { get; private set; }

    #endregion

    IServiceProvider ServiceProvider => Root?.GetServiceProvider();

    //public VosPersister(IRootVob root, SerializationOptions serializationOptions) : base(serializationOptions)
    //{
    //}
    public VosPersister(ILogger<VosPersister> logger, IVos vosRootManager, VosPersisterOptions options, SerializationOptions serializationOptions) : this(logger, vosRootManager, null, options, serializationOptions)
    {
    }
    public VosPersister(ILogger<VosPersister> logger, IVos vosRootManager, string rootName, VosPersisterOptions options, SerializationOptions serializationOptions)
        //: this(vosRootManager.Get(rootName), options?.SerializationOptions ?? serializationOptions)
        : base(options?.SerializationOptions ?? serializationOptions)
    {
        l = logger;
        Root = vosRootManager.Get(rootName);
        //this.RootName = rootName;
        //root = vosRootManager.Get(rootName);
    }


    public Task<IPersistenceResult> Exists<TValue>(IReferencable<IVobReference> referencable) => throw new System.NotImplementedException();


    public Func<RetrieveContext, Task>? BeforeRetrieve { get; set; }
    protected async Task OnBeforeRetrieve(RetrieveContext context)
    {
        BeforeRetrieve?.Invoke(context);

        // TODO: Dynamically attach this invokation somehow through some sort of event/callback plugin
        await ArchiveAdapterStatic.VosPersister_OnBeforeRetrieve(this, context);
    }

    public struct RetrieveContext
    {
        public Type? ListingType { get; set; }
        public IVob Vob { get; internal set; }
        public IReferencable<IVobReference> Referencable { get; internal set; }
        public HashSet<string> Flags { get; set; }
    }

    public async IAsyncEnumerable<IRetrieveResult<TValue>> RetrieveAll<TValue>(IReferencable<IVobReference> referencable)
    {
        //l.Trace($"{replaceMode.DescriptionString()} {obj?.GetType().Name} {replaceMode.ToArrow()} {fsReference}");

        //if (typeof(TValue) == typeof(Metadata<IEnumerable<Listing>>)) return (IRetrieveResult<TValue>)await List(referencable).ConfigureAwait(false);

        var vob = Root[referencable.Reference.Path];

        var result = new VosRetrieveResult<TValue>();

        bool anyMounts = false;
        var vobMounts = vob.AcquireNext<VobMounts>();

        Type metadataType = null;
        Type? listingType = null;

        if (typeof(TValue).IsGenericType && typeof(TValue).GetGenericTypeDefinition() == typeof(Metadata<>))
        {
            metadataType = typeof(TValue).GetGenericArguments()[0];

            if (metadataType.IsGenericType && metadataType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var enumerableType = metadataType.GetGenericArguments()[0];
                if (enumerableType.IsGenericType && enumerableType.GetGenericTypeDefinition() == typeof(Listing<>))
                {
                    listingType = enumerableType.GetGenericArguments()[0];
                    if (listingType == typeof(IArchive))
                    {
                    }
                }
            }
        }

        OnBeforeRetrieve(new RetrieveContext { Vob = vob, ListingType = listingType, Referencable = referencable });

        if (vobMounts != null)
        {
            // TODO: If TValue is IEnumerable, make a way (perhaps optional) to aggregate values from multiple ReadMounts.

            foreach (var mount in vobMounts.RankedEffectiveReadMounts)
            {
                var relativePathChunks = vob.PathElements.Skip(mount.VobDepth);
                var effectiveReference = !relativePathChunks.Any() ? mount.Target : mount.Target.GetChildSubpath(relativePathChunks);
                var rh = effectiveReference.GetReadHandle<TValue>(serviceProvider: ServiceProvider);

                var childResult = (await rh.Resolve().ConfigureAwait(false)).ToRetrieveResult();

                if (typeof(TValue) != typeof(object))
                {
                    var childType = childResult.Value?.GetType();
                    if (!childType.IsAssignableTo(typeof(TValue))) {
                        Trace.WriteLine($"Child {rh.Reference.Path} does not match type filter: {typeof(TValue).FullName}");
                        continue; 
                    }
                }

                if (childResult.IsFail()) result.Flags |= PersistenceResultFlags.Fail; // Indicates that at least one underlying persister failed

                if (childResult.IsSuccess == true)
                {
                    result.Flags |= PersistenceResultFlags.Success; // Indicates that at least one underlying persister succeeded

                    if (childResult.Flags.HasFlag(PersistenceResultFlags.Found))
                    {
                        result.Flags |= PersistenceResultFlags.Found;
                        result.Value = childResult.Value;
                        result.ResolvedVia = mount.Target;
                        l.Trace($"{result}: {vob.Path}");
                        yield return result;
                    }
                }
            }
        }
        if (!anyMounts) result.Flags |= PersistenceResultFlags.MountNotAvailable;
        l.Trace($"{result}: {vob.Path}");
        yield return result;
    }

    public async Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<IVobReference> referencable)
    {
        IRetrieveResult<TValue>? singleResult = null;

        await foreach (var multiResult in RetrieveAll<TValue>(referencable))
        {
            if (multiResult.IsSuccess == true)
            {
                if (multiResult.Flags.HasFlag(PersistenceResultFlags.MountNotAvailable))
                {
                    return multiResult;
                }
                continue;
            }

            if (singleResult != null && multiResult.IsSuccess == true)
            {
                return new RetrieveResult<TValue>
                {
                    Error = "Multiple mounts returned a value",
                    Flags = PersistenceResultFlags.Fail | PersistenceResultFlags.Found | PersistenceResultFlags.Ambiguous
                };
            }

            singleResult = multiResult;
        }

        return singleResult;
    }

    public Task<IPersistenceResult> Create<TValue>(IReferencable<IVobReference> referencable, TValue value) => throw new System.NotImplementedException();
    public Task<IPersistenceResult> Update<TValue>(IReferencable<IVobReference> referencable, TValue value) => throw new System.NotImplementedException();
    public async Task<IPersistenceResult> Upsert<TValue>(IReferencable<IVobReference> referencable, TValue value)
    {
        //l.Trace($"{ReplaceMode.Upsert.DescriptionString()} {value?.GetType().Name} {ReplaceMode.Upsert.ToArrow()} {referencable.Reference}");

        var vob = Root[referencable.Reference.Path];

        var result = new VosPersistenceResult();

        bool anyMounts = false;
        var vobMounts = vob.AcquireNext<VobMounts>();
        if (vobMounts != null)
        {
            foreach (var mount in vobMounts.RankedEffectiveWriteMounts)
            {
                var relativePathChunks = vob.PathElements.Skip(mount.VobDepth);
                var effectiveReference = !relativePathChunks.Any() ? mount.Target : mount.Target.GetChildSubpath(relativePathChunks);

                var wh = effectiveReference.GetWriteHandle<TValue>(serviceProvider: ServiceProvider);

                wh.Value = value;
                var childResult = (await wh.Put().ConfigureAwait(false)).ToPersistenceResult();

                if (childResult.IsFail()) result.Flags |= PersistenceResultFlags.Fail; // Indicates that at least one underlying persister failed

                if (childResult.IsSuccess == true)
                {
                    result.Flags |= PersistenceResultFlags.Success; // Indicates that at least one underlying persister succeeded
                    result.ResolvedVia = mount.Target;

                    l.Trace(result.ToString() + " " + wh.Reference);
                    return result;
                }
            }
        }
        if (!anyMounts)
        {
            result.Flags |= PersistenceResultFlags.MountNotAvailable;
            result.ResolvedVia = referencable?.Reference;
        }
        l.Trace($"{result.Flags}: {ReplaceMode.Upsert.DescriptionString()} {value?.GetType().Name} {ReplaceMode.Upsert.ToArrow()} {referencable.Reference} via {result.ResolvedVia}");
        return result;
    }
    public Task<IPersistenceResult> Delete(IReferencable<IVobReference> referencable)
    {
        l.Trace($"Delete xx> {referencable.Reference}");
        throw new System.NotImplementedException();
    }

    #region List


    // TODO REVIEW - is this redundant / old?  Use overload with filter param instead?
    public async Task<IRetrieveResult<IEnumerable<Listing<TValue>>>> List<TValue>(IReferencable<IVobReference> referencable)
    {
        var listingsLists = await RetrieveAll<Metadata<IEnumerable<Listing<TValue>>>>(referencable).ToListAsync();

        List<Listing<TValue>> listings = new();

        var consolidatedResult = new RetrieveResult<IEnumerable<Listing<TValue>>>();
        consolidatedResult.Value = listings;

        foreach (var result in listingsLists)
        {
            consolidatedResult.Flags |= result.Flags; // REVIEW - maybe only OR certain flags explicitly to prevent unintended consequences

            if (result.Value.Value != null)
            {
                listings.AddRange(result.Value.Value);
            }
        }
        return consolidatedResult;
    }

    public async Task<IRetrieveResult<IEnumerable<Listing<TValue>>>> List<TValue>(IReferencable<IVobReference> referencable, ListFilter filter = null)
    {
        l.Trace($"List ...> {referencable.Reference}");

        var retrieveResult = await Retrieve<Metadata<IEnumerable<Listing<TValue>>>>(referencable).ConfigureAwait(false);
        var result = retrieveResult.IsSuccess()
            ? RetrieveResult<IEnumerable<Listing<TValue>>>.Success(retrieveResult.Value.Value)
            : new RetrieveResult<IEnumerable<Listing<TValue>>> { Flags = retrieveResult.Flags, Error = retrieveResult.Error };
        l.Trace(result.ToString());
        return result;
        //    var vob = Root[referencable.Reference.Path];

        //    var result = new VosRetrieveResult<Metadata<IEnumerable<Listing>>>();

        //    bool anyMounts = false;
        //    var vobMounts = vob.AcquireNext<VobMounts>();
        //    if (vobMounts != null)
        //    {
        //        foreach (var mount in vobMounts.RankedEffectiveReadMounts)
        //        {
        //            var relativePathChunks = vob.PathElements.Skip(mount.VobDepth);
        //            var effectiveReference = !relativePathChunks.Any() ? mount.Target : mount.Target.GetChildSubpath(relativePathChunks);
        //            var rh = effectiveReference.GetReadHandle<Metadata<IEnumerable<Listing>>>(ServiceProvider);

        //            var childResult = (await rh.Resolve().ConfigureAwait(false)).ToRetrieveResult();

        //            if (childResult.IsFail()) result.Flags |= PersistenceResultFlags.Fail; // Indicates that at least one underlying persister failed

        //            if (childResult.IsSuccess == true)
        //            {
        //                result.Flags |= PersistenceResultFlags.Success; // Indicates that at least one underlying persister succeeded

        //                if (childResult.Flags.HasFlag(PersistenceResultFlags.Found))
        //                {
        //                    result.Value = childResult.Value;
        //                    result.ResolvedVia = mount.Target;
        //                    return result;
        //                }
        //            }
        //        }
        //    }
        //    if (!anyMounts) result.Flags |= PersistenceResultFlags.MountNotAvailable;
        //    return result;
    }

    #endregion

    private readonly ILogger l;
}
