using LionFire.IO;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Structures;
using LionFire.Vos;
using LionFire.Vos.Mounts;
using LionFire.Vos.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace LionFire.Persistence.Persisters.Vos;

public class VosPersister : SerializingPersisterBase<VosPersisterOptions>, IPersister<IVobReference>, IBatchingReadPersister<IVobReference>
{
    #region Dependencies

    IServiceProvider ServiceProvider => Root?.GetServiceProvider();

    #region Root

    public IRootVob Root { get; private set; }

    #endregion

    #endregion

    #region Configuration

    public VosPersisterOptions VosPersisterOptions { get; init; }

    #endregion

    #region Lifecycle

    public VosPersister(ILogger<VosPersister> logger, IVos vosRootManager, SerializationOptions serializationOptions, PersisterEvents persisterEvents, IOptionsMonitor<VosPersisterOptions> optionsMonitor)
        : this(logger, vosRootManager, serializationOptions, persisterEvents, optionsMonitor, OptionsName.Default)
    {
    }

    public VosPersister(ILogger<VosPersister> logger, IVos vosRootManager, SerializationOptions serializationOptions, PersisterEvents persisterEvents
        , IOptionsMonitor<VosPersisterOptions> optionsMonitor
        , OptionsName optionsName
        )
        : base(optionsMonitor.Get(optionsName.NameOrDefault)?.SerializationOptions ?? serializationOptions, persisterEvents)
    {
        l = logger;
        VosPersisterOptions = optionsMonitor.Get(optionsName.NameOrDefault);
        var rootName = VosPersisterOptions?.RootName ?? optionsName.Name;
        Root = vosRootManager.Get(rootName);
    }

    #endregion

    #region IBatchingReadPersister

    public async IAsyncEnumerable<IRetrieveResult<TValue>> RetrieveBatches<TValue>(IReferencable<IVobReference> referencable)
    {
        //l.Trace($"{replaceMode.DescriptionString()} {obj?.GetType().Name} {replaceMode.ToArrow()} {fsReference}");

        //if (typeof(TValue) == typeof(Metadata<IEnumerable<Listing>>)) return (IRetrieveResult<TValue>)await List(referencable).ConfigureAwait(false);

        var vob = Root[referencable.Reference.Path];

        var result = new VosRetrieveResult<TValue>();

        bool anyMounts = false;
        var vobMounts = vob.Acquire<VobMounts>();

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
                    //if (listingType == typeof(IArchive))
                    //{
                    //}
                }
            }
        }

        if (listingType != null)
        {
            var blea = new BeforeListEventArgs
            {
                Vob = vob,
                ResultType = typeof(TValue),
                Referencable = referencable,
                ListingType = listingType,
                Persister = this,
            };

            foreach (var handlers in vob.GetAcquireEnumerator2<IVob, Handlers<BeforeListEventArgs>>())
            {
                blea.HandlerVob = handlers.Item2;
                await handlers.Item1.Raise(blea).ConfigureAwait(false);
            }
        }
        //await OnBeforeRetrieve(new VosRetrieveContext { Persister = this, Vob = vob, ListingType = listingType, Referencable = referencable });


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
                    if (!childType.IsAssignableTo(typeof(TValue)))
                    {
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

    #endregion

    #region IReadPersister

    public Task<IPersistenceResult> Exists<TValue>(IReferencable<IVobReference> referencable) => throw new System.NotImplementedException();

    public async Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<IVobReference> referencable)
    {
        IRetrieveResult<TValue>? singleResult = null;

        await foreach (var multiResult in RetrieveBatches<TValue>(referencable))
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
    
    #endregion

    #region IWritePersister

    public Task<IPersistenceResult> Create<TValue>(IReferencable<IVobReference> referencable, TValue value) => throw new System.NotImplementedException();
    public Task<IPersistenceResult> Update<TValue>(IReferencable<IVobReference> referencable, TValue value) => throw new System.NotImplementedException();
    public async Task<IPersistenceResult> Upsert<TValue>(IReferencable<IVobReference> referencable, TValue value)
    {
        //l.Trace($"{ReplaceMode.Upsert.DescriptionString()} {value?.GetType().Name} {ReplaceMode.Upsert.ToArrow()} {referencable.Reference}");

        var vob = Root[referencable.Reference.Path];

        var result = new VosPersistenceResult();

        bool anyMounts = false;
        var vobMounts = vob.Acquire<VobMounts>();
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

    #endregion

    #region IListPersister

    // TODO REVIEW - is this redundant / old?  Use overload with filter param instead?
    public async Task<IRetrieveResult<IEnumerable<Listing<TValue>>>> List<TValue>(IReferencable<IVobReference> referencable)
    {
        var listingsLists = await RetrieveBatches<Metadata<IEnumerable<Listing<TValue>>>>(referencable).ToListAsync();

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

    #region Misc

    private readonly ILogger l;

    #endregion

}
