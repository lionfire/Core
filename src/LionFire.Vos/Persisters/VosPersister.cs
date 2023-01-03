using LionFire.Execution;
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
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading;
using static LionFire.Persistence.Persisters.Vos.VosPersister;

namespace LionFire.Persistence.Persisters.Vos;

// TODO FIXME: also do RetrieveOpMounts approach on non-Retrieve operations, to avoid infinite loops. (Or avoid some other way)

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

    #region Metrics

    private static readonly Meter Meter = new("LionFire.Vos", "1.0");
    private static readonly Counter<long> ExistsC = Meter.CreateCounter<long>("Exists");
    private static readonly Counter<long> RetrieveC = Meter.CreateCounter<long>("Retrieve");
    private static readonly Counter<long> RetrieveRetryAfterMountsChangedC = Meter.CreateCounter<long>("Retrieve.RetryAfterMountsChanged");
    private static readonly Counter<long> RetrieveBatchC = Meter.CreateCounter<long>("Retrieve.Batch");
    private static readonly Counter<long> ListC = Meter.CreateCounter<long>("List");

    #endregion

    #region IBatchingReadPersister

    static AsyncLocal<List<IMount>> RetrieveOpMounts = new();

    public async IAsyncEnumerable<IRetrieveResult<TValue>> RetrieveBatches<TValue>(IReferencable<IVobReference> referencable)
    {
        //l.Trace($"{replaceMode.DescriptionString()} {obj?.GetType().Name} {replaceMode.ToArrow()} {fsReference}");

        //if (typeof(TValue) == typeof(Metadata<IEnumerable<Listing>>)) return (IRetrieveResult<TValue>)await List(referencable).ConfigureAwait(false);

        var vob = Root[referencable.Reference.Path];


        bool anyMounts = false;

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

        #region Retry control 
        VobMounts? oldVobMounts = null;
        int maxRetries = 10;
        int iteration = 0;
        #endregion

        var resultForNoChildResults = new VosRetrieveResult<TValue>();
    //for (; iteration < maxRetries; iteration++) { }
    again:
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
        else
        {
            var blea = new BeforeRetrieveEventArgs
            {
                Vob = vob,
                ResultType = typeof(TValue),
                Referencable = referencable,
                Persister = this,
            };

            foreach (var handlers in vob.GetAcquireEnumerator2<IVob, Handlers<BeforeRetrieveEventArgs>>())
            {
                blea.HandlerVob = handlers.Item2;
                await handlers.Item1.Raise(blea).ConfigureAwait(false);
            }
        }
        //await OnBeforeRetrieve(new VosRetrieveContext { Persister = this, Vob = vob, ListingType = listingType, Referencable = referencable });

        var (vobMountsNode, mounts) = GetMounts();

        (VobMounts vobMountsNode, IEnumerable<IMount> mounts) GetMounts()
        {
            var vobMountsNode = vob.Acquire<VobMounts>();

            IEnumerable<IMount> mounts = null;

            if (vobMountsNode.Vob.Key == oldVobMounts?.Vob.Key)
            {
                l.LogDebug($"UNTESTED - Second pass retrieve, skipping {oldVobMounts.RankedEffectiveReadMounts.Count()} previous mounts");
                l.LogDebug($"TEMP - Old mounts:");
                {
                    var sb = new StringBuilder();
                    foreach (var mount in oldVobMounts.RankedEffectiveReadMounts)
                    {
                        sb.Append(" - ");
                        sb.AppendLine(mount.ToString());
                    }
                    l.LogDebug(sb.ToString());

                    sb = new StringBuilder();
                    l.LogDebug($"TEMP - New mounts:");
                    foreach (var mount in vobMountsNode.RankedEffectiveReadMounts)
                    {
                        sb.Append(" - ");
                        sb.AppendLine(mount.ToString());
                    }
                    l.LogDebug(sb.ToString());

                    sb = new StringBuilder();
                    l.LogDebug($"TEMP - Continuing with mounts:");
                    mounts = vobMountsNode.RankedEffectiveReadMounts.Except(oldVobMounts.RankedEffectiveReadMounts);
                    foreach (var mount in mounts)
                    {
                        sb.Append(" - ");
                        sb.AppendLine(mount.ToString());
                    }
                    l.LogDebug(sb.ToString());
                }
            }
            else
            {
                if (oldVobMounts != null)
                {
                    l.LogDebug($"[retrieving {referencable.Reference.Path}({typeof(TValue).Name})] Second pass: different VobMounts node.  Old @ {oldVobMounts.Vob.Key}, New @ {vobMountsNode.Vob.Key}");
                }
                //else
                //{
                //    l.LogDebug($"[retrieving {referencable.Reference.Path}({typeof(TValue).Name})] First pass: {vobMountsNode.RankedEffectiveReadMounts.Count()} mounts");
                //}
                mounts = vobMountsNode.RankedEffectiveReadMounts;
            }
            return (vobMountsNode, mounts);
        }

        if (mounts?.Any() == true)
        {
            // TODO: If TValue is IEnumerable, make a way (perhaps optional) to aggregate values from multiple ReadMounts.
            {
                var sb = new StringBuilder();
                sb.Append($"[retrieving {referencable.Reference.Path}({typeof(TValue).Name})] from mounts:");
                if (vobMountsNode.ReadMountsVersion > 0)
                {
                    sb.Append("v:");
                    sb.Append(vobMountsNode.ReadMountsVersion);
                }
                l.Trace(sb.ToString());
            }
            foreach (var mount in mounts)
            {
                #region Prevent recursion
                if (RetrieveOpMounts.Value != null)
                {
                    if (RetrieveOpMounts.Value.Contains(mount))
                    {
                        //Trace.Write("RetrieveOpMounts.Value.Contains(mount)");
                        continue;
                    }
                }
                #endregion
                l.Trace($"[retrieving {referencable.Reference.Path}({typeof(TValue).Name})] - retrieving from {mount}");

                anyMounts = true;

                var relativePathChunks = vob.PathElements.Skip(mount.VobDepth);
                var effectiveReference = !relativePathChunks.Any() ? mount.Target : mount.Target.GetChildSubpath(relativePathChunks);
                var rh = effectiveReference.GetReadHandle<TValue>(serviceProvider: ServiceProvider);

                RetrieveOpMounts.Value ??= new();
                RetrieveOpMounts.Value.Add(mount);

                IRetrieveResult<TValue> childResult;
                try
                {
                    childResult = (await rh.Resolve().ConfigureAwait(false)).ToRetrieveResult();
                    l.Info(childResult.ToString() + " " + rh.Reference + $" (for {referencable.Reference})"); // TEMP log level
                }
                finally
                {
                    RetrieveOpMounts.Value.Remove(mount);
                }

                if (typeof(TValue) != typeof(object) && childResult.HasValue)
                {
                    var childType = childResult.Value?.GetType();
                    if (!childType.IsAssignableTo(typeof(TValue)))
                    {
                        Trace.WriteLine($"Child {rh.Reference.Path} does not match type filter: {typeof(TValue).FullName}");
                        continue;
                    }
                }

                if (childResult.IsFail()) resultForNoChildResults.Flags |= PersistenceResultFlags.Fail; // Indicates that at least one underlying persister failed

                if (childResult.IsSuccess == true)
                {
                    resultForNoChildResults.Flags |= PersistenceResultFlags.Success; // Indicates that at least one underlying persister succeeded

                    if (childResult.Flags.HasFlag(PersistenceResultFlags.Found))
                    {
                        l.LogTrace($"[retrieving {referencable.Reference.Path}({typeof(TValue).Name})] Found child.  {(oldVobMounts == null ? "First" : "Second (UNTESTED)")} pass retrieve @ {referencable.Reference}({typeof(TValue).Name})");

                        resultForNoChildResults.Flags |= PersistenceResultFlags.Found;
                        yield return childResult;
                    }
                }
            }
        }

        bool MountsEqual(IEnumerable<IMount> left, IEnumerable<IMount> right)
        {
            if(left.Count() != right.Count()) return false;

            var rightEnumerator = right.GetEnumerator();
            foreach(var leftItem in left)
            {
                rightEnumerator.MoveNext();
                if(leftItem.ToString() != rightEnumerator.Current.ToString()) return false;
            }
            return true;
        }

        if (!resultForNoChildResults.Flags.HasFlag(PersistenceResultFlags.Found))
        {

            bool detectedMountsChange = false;

            {
                var eventArgs = new AfterNotFoundEventArgs
                {
                    Vob = vob,
                    ResultType = typeof(TValue),
                    Referencable = referencable,
                    Persister = this,
                    FoundMore = false, // (Output)
                };

                foreach (var handlers in vob.GetAcquireEnumerator2<IVob, Handlers<AfterNotFoundEventArgs>>())
                {
                    //oldVobMounts ??= vobMountsNode;
                    eventArgs.HandlerVob = handlers.Item2;
                    await handlers.Item1.Raise(eventArgs).ConfigureAwait(false);
                }
                detectedMountsChange |= eventArgs.FoundMore;
            }

            if (!detectedMountsChange && iteration < maxRetries)
            {
                var (newVobMountsNode, newMounts) = GetMounts();
                if (newVobMountsNode != vobMountsNode || !MountsEqual(mounts, newMounts))
                {
                    detectedMountsChange |= true;
                }
            }

            if (detectedMountsChange && iteration < maxRetries)
            {
                RetrieveRetryAfterMountsChangedC.Add(1);
                iteration++;
                l.LogDebug($"[retrieving {referencable.Reference.Path}({typeof(TValue).Name})] FOUNDMORE - Trying retrieve #{(iteration + 1)}");
                goto again;
            }

            if (!anyMounts)
            {
                resultForNoChildResults.Flags |= PersistenceResultFlags.MountNotAvailable;
                l.Info(resultForNoChildResults.ToString() + $" (for {referencable.Reference}) "); // TEMP log level
            }
            else
            {
                resultForNoChildResults.Flags |= PersistenceResultFlags.NotFound;
            }
            l.Trace($"{resultForNoChildResults}: {vob.Path}");

            var sb = new StringBuilder($"[retrieve  {referencable.Reference.Path}({typeof(TValue).Name})] NO CHILD f:[{resultForNoChildResults.Flags}]" + Environment.NewLine);
            foreach (var mount in mounts)
            {
                sb.Append(" - ");
                sb.AppendLine(mount.ToString());
            }
            l.Trace(sb.ToString());
            l.Trace($"[retrieve {referencable.Reference.Path}({typeof(TValue).Name})] End. V:{vobMountsNode.ReadMountsVersion} #:{vobMountsNode.RankedEffectiveReadMounts.Count()}");
            yield return resultForNoChildResults;
        }
    }

    #endregion

    #region IReadPersister

    public Task<IPersistenceResult> Exists<TValue>(IReferencable<IVobReference> referencable)
    {
        ExistsC.Add(1);
        throw new System.NotImplementedException();
    }

#nullable enable
    public static Type? GetMetadataListingType<TValue>()
    {
        if (typeof(TValue).IsConstructedGenericType)
        {
            var generic = typeof(TValue).GetGenericTypeDefinition();
            if (generic == typeof(Metadata<>))
            {
                var genericParameter = typeof(TValue).GetGenericArguments()[0];
                if (genericParameter.IsConstructedGenericType)
                {
                    var metadataGeneric = genericParameter.GetGenericTypeDefinition();
                    if (metadataGeneric == typeof(Listing<>))
                    {
                        var listingType = genericParameter.GetGenericArguments()[0];
                        return listingType;
                        //var aggregatorType = typeof(ResultAggregator<,>).MakeGenericType(typeof(TValue), typeof(listingType));
                    }
                }
            }
        }
        return null;
    }
    public static class ResultAggregator<TValue, TItem>
    {
        //public Func<TValue, >
        public static async Task<IRetrieveResult<TValue>> RetrieveWithAggregation<TValue, TItem>(IReferencable<IVobReference> referencable)
        {
            List<TItem> aggregation = new List<TItem>();

            throw new NotImplementedException();
        }
    }

    public bool CanAggregateType<TValue>()
    {
        return GetMetadataListingType<TValue>() != null;
    }

    protected async Task<IRetrieveResult<TValue>> RetrieveWithAggregation<TValue>(IReferencable<IVobReference> referencable)
    {
        throw new NotImplementedException();
#warning NEXT: Get aggregator
        IRetrieveResult<TValue>? singleResult = null;

        await foreach (var multiResult in RetrieveBatches<TValue>(referencable))
        {
            //if (multiResult.IsSuccess == true)
            //{
            //    if (multiResult.Flags.HasFlag(PersistenceResultFlags.MountNotAvailable))
            //    {
            //        return multiResult;
            //    }
            //    continue;
            //}
            if (multiResult.IsSuccess != true)
            {
                if (multiResult.Flags.HasFlag(PersistenceResultFlags.MountNotAvailable))
                {
                    continue;
                }
                else return multiResult; // Return the failure
            }

            if (singleResult != null && multiResult.IsSuccess == true)
            {
                l.Error($"AMBIGUOUS: {referencable.Reference} {typeof(TValue).Name}");
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

    public async Task<IRetrieveResult<TValue>> Retrieve<TValue>(IReferencable<IVobReference> referencable)
    {
        RetrieveC.Add(1);
        if (CanAggregateType<TValue>()) { return await RetrieveWithAggregation<TValue>(referencable).ConfigureAwait(false); }

        IRetrieveResult<TValue>? singleResult = null;

        await foreach (var multiResult in RetrieveBatches<TValue>(referencable))
        {
            RetrieveBatchC.Add(1);
            //if (multiResult.IsSuccess == true)
            //{
            //    if (multiResult.Flags.HasFlag(PersistenceResultFlags.MountNotAvailable))
            //    {
            //        return multiResult;
            //    }
            //    continue;
            //}
            if (multiResult.IsSuccess != true)
            {
                if (multiResult.Flags.HasFlag(PersistenceResultFlags.MountNotAvailable))
                {
                    continue;
                }
                else return multiResult; // Return the failure
            }

            if (singleResult != null && multiResult.IsSuccess == true)
            {
                l.Error($"AMBIGUOUS: {referencable.Reference} {typeof(TValue).Name}");
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
                    result.ResolvedViaMount = mount;

                    l.Trace(result.ToString() + " " + wh.Reference);
                    return result;
                }
            }
        }
        if (!anyMounts)
        {
            result.Flags |= PersistenceResultFlags.MountNotAvailable;
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
        ListC.Add(1);
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
        //                    result.ResolvedViaMount = mount;
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
