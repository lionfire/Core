#nullable enable
using LionFire.Dependencies;
using LionFire.Execution;
using LionFire.Extensions.DefaultValues;
using LionFire.ExtensionMethods.Acquisition;
using LionFire.IO;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Structures;
using LionFire.Vos;
using LionFire.Vos.Mounts;
using LionFire.Vos.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using static LionFire.Persistence.Persisters.Vos.VosPersister;
using LionFire.Data;
using LionFire.Results;

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

    public VosPersister(ILogger<VosPersister> logger, IVos vosRootManager, SerializationOptions serializationOptions, PersisterEvents persisterEvents, IOptionsMonitor<VosPersisterOptions> optionsMonitor, IServiceProvider serviceProvider)
        : this(logger, vosRootManager, serializationOptions, persisterEvents, optionsMonitor, OptionsName.Default, serviceProvider)
    {
    }

    public VosPersister(ILogger<VosPersister> logger, IVos vosRootManager, SerializationOptions serializationOptions, PersisterEvents persisterEvents
        , IOptionsMonitor<VosPersisterOptions> optionsMonitor
        , OptionsName optionsName
        , IServiceProvider serviceProvider
        )
        : base(serviceProvider, optionsMonitor.Get(optionsName.NameOrDefault)?.SerializationOptions ?? serializationOptions, persisterEvents)
    {
        l = logger;
        VosPersisterOptions = optionsMonitor.Get(optionsName.NameOrDefault);
        var rootName = VosPersisterOptions?.RootName ?? optionsName.Name;
        Root = vosRootManager.Get(rootName) ?? throw new ArgumentException($"Root not found: {rootName}");
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

    public async IAsyncEnumerable<IGetResult<TValue>> RetrieveBatches<TValue>(IReferenceable<IVobReference> referencable, RetrieveOptions? options = null)
    {
        var finishAfterReturningFirstFound = (options ?? RetrieveOptions.Default).ReturnFirstFound;

        //l.Trace($"{replaceMode.DescriptionString()} {obj?.GetType().Name} {replaceMode.ToArrow()} {fsReference}");

        //if (typeof(TValue) == typeof(Metadata<IEnumerable<Listing>>)) return (IGetResult<TValue>)await List(referencable).ConfigureAwait(false);

        var vob = Root[referencable.Reference.Path];


        bool anyMounts = false;

        Type? metadataType = null;
        Type? listingItemType = null;

        if (typeof(TValue).IsGenericType && typeof(TValue).GetGenericTypeDefinition() == typeof(Metadata<>))
        {
            metadataType = typeof(TValue).GetGenericArguments()[0];

            if (metadataType.IsGenericType && metadataType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var enumerableType = metadataType.GetGenericArguments()[0];
                if (enumerableType.IsGenericType && enumerableType.GetGenericTypeDefinition() == typeof(IListing<>))
                {
                    listingItemType = enumerableType.GetGenericArguments()[0];
                    //if (listingItemType == typeof(IArchive))
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

        if (listingItemType != null)
        {
#warning NEXT: Add Vos children (that have mounts) to the list here.  

            if (listingItemType != typeof(object))
            {
                //throw new NotImplementedException();
            }
            else
            {
                var vobChildren = vob.Children;

                if (listingItemType == typeof(object))
                {
                    var listingType = typeof(Listing<>).MakeGenericType(listingItemType);

                    var vobChildrenListings = vobChildren.Select(v =>
                    {
                        var listing = (IListing<object> /* TODO HARDCAST */)Activator.CreateInstance(listingType, v.Key)!;
#warning NEXT: Don't set this to true if it's not really a directory.  TODO: How to tell? Children.Any()?  Or NonMetaChildren.Any()?
                        listing.IsDirectory = v.Value.Children.Any();
                        return listing;
                    });

                    yield return new VosRetrieveResult<TValue>((TValue)(object)new Metadata<IEnumerable<IListing<object>>>(vobChildrenListings));
                }
                else
                {
                    // TODO get all known ReadHandles for child, and skip ones that do not match listingItemType
                    //vobChildren = vobChildren.Where(v => v.Matches(listingItemType));
                }
            }
        }

    again:
        if (listingItemType != null)
        {
            var blea = new BeforeListEventArgs
            {
                Vob = vob,
                ResultType = typeof(TValue),
                Referenceable = referencable,
                ListingType = listingItemType,
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
                Referenceable = referencable,
                Persister = this,
            };

            foreach (var handlers in vob.GetAcquireEnumerator2<IVob, Handlers<BeforeRetrieveEventArgs>>())
            {
                blea.HandlerVob = handlers.Item2;
                await handlers.Item1.Raise(blea).ConfigureAwait(false);
            }
        }
        //await OnBeforeRetrieve(new VosRetrieveContext { Persister = this, Vob = vob, ListingType = listingItemType, Referenceable = referencable });

        var (vobMountsNode, mounts) = GetMounts();

        (VobMounts vobMountsNode, IEnumerable<IMount> mounts) GetMounts()
        {
            //var vobMountsNode = vob.Acquire<VobMounts>();
            var vobMountsNode = vob.GetOrAddNextVobNode<VobMounts>();
            var vobMounts = vobMountsNode.Value;
            //var vobMountsNode = IVobInternalsVobNodeExtensions.GetOrAddNextVobNode<VobMounts>(vob);
            IEnumerable<IMount> mounts = null;

            if (vobMounts.Vob.Key == oldVobMounts?.Vob.Key)
            {
#if DEBUG_Off
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
                    foreach (var mount in vobMounts.RankedEffectiveReadMounts)
                    {
                        sb.Append(" - ");
                        sb.AppendLine(mount.ToString());
                    }
                    l.LogDebug(sb.ToString());

                    sb = new StringBuilder();
                    l.LogDebug($"TEMP - Continuing with mounts:");
#endif
                mounts = vobMounts.RankedEffectiveReadMounts.Except(oldVobMounts.RankedEffectiveReadMounts);
#if DEBUG_Off
                    foreach (var mount in mounts)
                    {
                        sb.Append(" - ");
                        sb.AppendLine(mount.ToString());
                    }
                    l.LogDebug(sb.ToString());
                }
#endif
            }
            else
            {
                if (oldVobMounts != null)
                {
                    l.LogDebug($"[retrieving {referencable.Reference.Path}({typeof(TValue).Name})] Second pass: different VobMounts node.  Old @ {oldVobMounts.Vob.Key}, New @ {vobMounts.Vob.Key}");
                }
                //else
                //{
                //    l.LogDebug($"[retrieving {referencable.Reference.Path}({typeof(TValue).Name})] First pass: {vobMountsNode.RankedEffectiveReadMounts.Count()} mounts");
                //}
                mounts = vobMounts.RankedEffectiveReadMounts;
            }
            return (vobMounts, mounts);
        }

        if (mounts?.Any() == true)
        {
            // TODO: If TValue is IEnumerable, make a way (perhaps optional) to aggregate values from multiple ReadMounts.
            #region Log
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
            #endregion
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
                var rh = effectiveReference.GetReadHandle<TValue>(/*serviceProvider: ServiceProvider*/);

                RetrieveOpMounts.Value ??= new();
                RetrieveOpMounts.Value.Add(mount);

                IGetResult<TValue> childResult;
                try
                {
                    childResult = (await rh.Get().ConfigureAwait(false));
                    l.Info(childResult?.ToString() + " " + rh.Reference + $" (for {referencable.Reference})"); // TEMP log level
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

                if (childResult.IsFail()) resultForNoChildResults.Flags |= TransferResultFlags.Fail; // Indicates that at least one underlying persister failed

                if (childResult.IsSuccess == true)
                {
                    resultForNoChildResults.Flags |= TransferResultFlags.Success; // Indicates that at least one underlying persister succeeded

                    if (childResult.Flags.HasFlag(TransferResultFlags.Found))
                    {
                        l.LogTrace($"[retrieving {referencable.Reference.Path}({typeof(TValue).Name})] Found child.  {(oldVobMounts == null ? "First" : "Second (UNTESTED)")} pass retrieve @ {referencable.Reference}({typeof(TValue).Name})");

                        resultForNoChildResults.Flags |= TransferResultFlags.Found;
                        yield return childResult;
                        if (finishAfterReturningFirstFound && childResult.HasValue)
                        {
                            break;
                        }
                    }
                }
            }
        }

        bool MountsEqual(IEnumerable<IMount> left, IEnumerable<IMount> right)
        {
            if (left.Count() != right.Count()) return false;

            var rightEnumerator = right.GetEnumerator();
            foreach (var leftItem in left)
            {
                rightEnumerator.MoveNext();
                if (leftItem.ToString() != rightEnumerator.Current.ToString()) return false;
            }
            return true;
        }

        if (!resultForNoChildResults.Flags.HasFlag(TransferResultFlags.Found))
        {

            bool detectedMountsChange = false;

            {
                var eventArgs = new AfterNotFoundEventArgs
                {
                    Vob = vob,
                    ResultType = typeof(TValue),
                    Referenceable = referencable,
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
                RetrieveRetryAfterMountsChangedC.IncrementWithContext();
                iteration++;
                l.LogDebug($"[retrieving {referencable.Reference.Path}({typeof(TValue).Name})] FOUNDMORE - Trying retrieve #{(iteration + 1)}");
                goto again;
            }

            if (!anyMounts)
            {
                resultForNoChildResults.Flags |= TransferResultFlags.MountNotAvailable;
                l.Info(resultForNoChildResults.ToString() + $" (for {referencable.Reference}) "); // TEMP log level
            }
            else
            {
                resultForNoChildResults.Flags |= TransferResultFlags.NotFound;
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

    public Task<ITransferResult> Exists<TValue>(IReferenceable<IVobReference> referencable)
    {
        ExistsC.IncrementWithContext();
        throw new System.NotImplementedException();
    }

#nullable enable

    public static Type? GetMetadataListingType<TValue>() => typeof(TValue).UnwrapGeneric(typeof(Metadata<>))?.UnwrapGeneric(typeof(IEnumerable<>))?.IfGenericOrDefault(typeof(IListing<>));

    public static Type? GetMetadataListingItemType<TValue>() => GetMetadataListingType<TValue>()?.GetGenericArguments()[0];

    protected async Task<IGetResult<TValue>> RetrieveWithAggregation<TValue>(IReferenceable<IVobReference> referencable, RetrieveOptions? options = null)
    {
        RetrieveResult<TValue> aggregatedResult = new RetrieveResult<TValue>();
        List<IGetResult<TValue>> childResults = null;

        var aggregationItemType = WrapperUtils<TValue>.GetInnermostEnumerableItemType;
        if (aggregationItemType == null) throw new ArgumentException("Aggregation not supported for type: " + typeof(TValue).FullName);

        IListAggregator<TValue> aggregator = ListAggregator.Create<TValue>();

        await foreach (var childResult in RetrieveBatches<TValue>(referencable))
        {
            Debug.Assert(childResult != null);
            RetrieveBatchC.IncrementWithContext();
            childResults ??= new();

            childResults.Add(childResult);

            if (childResult.Flags.HasFlag(TransferResultFlags.Fail)) { aggregatedResult.Flags |= TransferResultFlags.InnerFail | TransferResultFlags.Fail; }
            if (childResult.Flags.HasFlag(TransferResultFlags.Success)) { aggregatedResult.Flags |= TransferResultFlags.Success; }
            if (childResult.Flags.HasFlag(TransferResultFlags.Found)) { aggregatedResult.Flags |= TransferResultFlags.Found; }

            if (childResult.HasValue) { aggregator.Aggregate(childResult.Value); }
        }

        if (!aggregatedResult.Flags.HasFlag(TransferResultFlags.Found)) { aggregatedResult.Flags |= TransferResultFlags.NotFound; }

        aggregatedResult.Value = aggregator.GetValue();

        aggregatedResult.InnerResult = childResults;
        return aggregatedResult;
    }

    #region Aggregator MOVE


    public bool CanAggregateType<TValue>() => WrapperUtils<TValue>.GetInnermostEnumerableType != null;
    //return GetMetadataListingItemType<TValue>() != null;

    private interface IListAggregator<TValue>
    {
        void Aggregate(TValue value);

        TValue GetValue();
    }

    private static class ListAggregator
    {
        public static IListAggregator<TValue> Create<TValue>()
            => (IListAggregator<TValue>)Activator.CreateInstance(typeof(WrappedEnumerableAggregator<,>).MakeGenericType(typeof(TValue), GetMetadataListingType<TValue>()));
    }

    private class WrappedEnumerableAggregator<TValue, TItem> : IListAggregator<TValue>
    {
        List<TItem>? aggregatedItems;

        public void Aggregate(TValue value)
        {
            aggregatedItems ??= new();
            aggregatedItems.AddRange(WrapperUtils<TValue>.GetEnumerable<TItem>(value));
        }

        public TValue GetValue() => WrapperUtils<TValue>.Wrap(aggregatedItems);
    }

    public static class WrapperUtils<T>
    {
        public static bool IsReadWrapper => isReadWrapper ??= typeof(T).GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IReadWrapper<>)).Any();
        private static bool? isReadWrapper;

        public static bool IsWriteWrapper => isWriteWrapper ??= typeof(T).GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IWriteWrapper<>)).Any();
        private static bool? isWriteWrapper;

        public static bool IsWrapper => IsReadWrapper && IsWriteWrapper;

        public static Type? GetWrappedReadType => getWrappedReadType ??= (Type?)typeof(T).GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IReadWrapper<>)).FirstOrDefault()?.GetGenericArguments()[0];
        private static Type? getWrappedReadType;

        public static Type? GetWrappedWriteType => getWrappedWriteType ??= (Type?)typeof(T).GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IWriteWrapper<>)).FirstOrDefault()?.GetGenericArguments()[0];
        private static Type? getWrappedWriteType;

        public static Type? GetInnermostWrappedReadType
        {
            get
            {
                if (wrappedType == null)
                {
                    for (wrappedType = GetWrappedReadType; wrappedType != null; wrappedType = GetWrappedReadType)
                    {
                        var child = typeof(WrapperUtils<>).MakeGenericType(wrappedType);
                        Type? innerWrappedReadType = (Type?)child!.GetProperty(nameof(GetWrappedReadType), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!.GetValue(null);
                        if (innerWrappedReadType != null)
                        {
                            wrappedType = innerWrappedReadType;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                return wrappedType;
            }
        }
        private static Type? wrappedType;

        public static Type? GetInnermostEnumerableType
        {
            get
            {
                getInnermostEnumerableType ??= GetInnermostWrappedReadType?.IsAssignableTo(typeof(IEnumerable)) == true ? GetInnermostWrappedReadType : GetInnermostWrappedReadType?.GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)).FirstOrDefault() ?? typeof(DBNull);

                return getInnermostEnumerableType == typeof(DBNull) ? null : getInnermostEnumerableType;
            }
        }
        private static Type? getInnermostEnumerableType;

        public static Type? GetInnermostEnumerableItemType() => GetInnermostEnumerableType?.GetGenericArguments()[0];

        public static IEnumerable<TItem>? GetEnumerable<TItem>(T wrapper)
        {
            if(wrapper is IReadWrapper<IEnumerable<TItem>> w)
            {
                return w.Value;
            }
            return null;
        }

        public static T Wrap<TInner>(TInner value)
        {
            // ENH: If there isn't a ctor that takes a single parameter of type TInner, create using default constructor, and set via IWriteWrapper<TInner>
            return (T)Activator.CreateInstance(typeof(T), value);
        }
    }
    #endregion

    public async Task<IGetResult<TValue>> Retrieve<TValue>(IReferenceable<IVobReference> referencable, RetrieveOptions? options = null)
    {
        if (CanAggregateType<TValue>()) { return await RetrieveWithAggregation<TValue>(referencable).ConfigureAwait(false); }

        bool returnFirstSuccess = options?.ValidationFlags.HasFlag(RetrieveFlags.FirstSuccess) == true;
        RetrieveC.IncrementWithContext();

        IGetResult<TValue>? singleResult = null;

        IGetResult<TValue>? singleNonSuccessResult = null;
        List<IGetResult<TValue>>? nonSuccessResults = null;

        await foreach (var childResult in RetrieveBatches<TValue>(referencable))
        {
            Debug.Assert(childResult != null);
            RetrieveBatchC.IncrementWithContext();

            #region Non-success
            if (childResult.IsSuccess != true)
            {
                if (singleNonSuccessResult != null && nonSuccessResults == null)
                {
                    nonSuccessResults = new();
                    nonSuccessResults.Add(singleNonSuccessResult);
                }
                if (nonSuccessResults != null)
                {
                    nonSuccessResults.Add(childResult);
                }
                singleNonSuccessResult ??= childResult;

                //if (ShouldReturnSingleErrorImmediately(childResult)) return childResult; // Return the failure
                continue;
            }
            #endregion

            #region Ambiguous check
            if (singleResult != null && childResult.IsSuccess == true)
            {
                l.Error($"AMBIGUOUS: {referencable.Reference} {typeof(TValue).Name}");
                return new RetrieveResult<TValue>
                {
                    Error = "Multiple mounts returned a value",
                    Flags = TransferResultFlags.Fail | TransferResultFlags.Found | TransferResultFlags.Ambiguous
                };
            }
            #endregion

            #region returnFirstSuccess
            if (returnFirstSuccess && singleResult?.IsSuccess == true) return singleResult;
            #endregion

            singleResult = childResult;
        }

        if (singleResult is not null) { return singleResult; }
        else if (singleNonSuccessResult is not null) return singleNonSuccessResult;
        else if (nonSuccessResults is not null) return new RetrieveResult<TValue> { Flags = TransferResultFlags.Fail | TransferResultFlags.InnerFail, InnerResults = nonSuccessResults };
        else throw new UnreachableCodeException($"{nameof(RetrieveBatches)} must return at least one result");

        //bool ShouldReturnSingleErrorImmediately(IGetResult<TValue> r) => r.Flags.HasFlag(TransferResultFlags.MountNotAvailable);
    }

    #endregion

    #region IWritePersister

    public Task<ITransferResult> Create<TValue>(IReferenceable<IVobReference> referencable, TValue value) => throw new System.NotImplementedException();
    public Task<ITransferResult> Update<TValue>(IReferenceable<IVobReference> referencable, TValue value) => throw new System.NotImplementedException();
    public async Task<ITransferResult> Upsert<TValue>(IReferenceable<IVobReference> referencable, TValue value)
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
                var childResult = (await wh.Set().ConfigureAwait(false));

                if (childResult.IsFail()) result.Flags |= TransferResultFlags.Fail; // Indicates that at least one underlying persister failed

                if (childResult.IsSuccess == true)
                {
                    result.Flags |= TransferResultFlags.Success; // Indicates that at least one underlying persister succeeded
                    result.ResolvedViaMount = mount;

                    l.Trace(result.ToString() + " " + wh.Reference);
                    return result;
                }
            }
        }
        if (!anyMounts)
        {
            result.Flags |= TransferResultFlags.MountNotAvailable;
        }
        l.Trace($"{result.Flags}: {ReplaceMode.Upsert.DescriptionString()} {value?.GetType().Name} {ReplaceMode.Upsert.ToArrow()} {referencable.Reference} via {result.ResolvedVia}");
        return result;
    }
    public Task<ITransferResult> DeleteReferenceable(IReferenceable<IVobReference> referencable)
    {
        l.Trace($"Delete xx> {referencable.Reference}");
        throw new System.NotImplementedException();
    }

    #endregion

    #region IListPersister

    // TODO - use a single overload, with null defaulting ListOptions

    //public async Task<IGetResult<IEnumerable<IListing<TValue>>>> List<TValue>(IReferenceable<IVobReference> referencable)
    //{
    //    throw new NotSupportedException();
    //    ListC.IncrementWithContext();
    //    var listingsLists = await RetrieveBatches<Metadata<IEnumerable<IListing<TValue>>>>(referencable).ToListAsync();

    //    List<IListing<TValue>> listings = new();

    //    var consolidatedResult = new RetrieveResult<IEnumerable<IListing<TValue>>>();
    //    consolidatedResult.Value = listings;

    //    foreach (var result in listingsLists)
    //    {
    //        consolidatedResult.Flags |= result.Flags; // REVIEW - maybe only OR certain flags explicitly to prevent unintended consequences

    //        if (result.Value.Value != null)
    //        {
    //            listings.AddRange(result.Value.Value);
    //        }
    //    }

    //    return consolidatedResult;
    //}

    public async Task<IGetResult<IEnumerable<IListing<TValue>>>> List<TValue>(IReferenceable<IVobReference> referencable, ListFilter filter = null)
    {
        l.Trace($"List ...> {referencable.Reference}");

        var retrieveResult = await RetrieveWithAggregation<Metadata<IEnumerable<IListing<TValue>>>>(referencable).ConfigureAwait(false);
        var result = retrieveResult.IsSuccess()
            ? RetrieveResult<IEnumerable<IListing<TValue>>>.Success(retrieveResult.Value.Value)
            : new RetrieveResult<IEnumerable<IListing<TValue>>> { Flags = retrieveResult.Flags, Error = (retrieveResult as IErrorResult)?.Error };
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

        //            var childResult = (await rh.Get().ConfigureAwait(false));

        //            if (childResult.IsFail()) result.Flags |= TransferResultFlags.Fail; // Indicates that at least one underlying persister failed

        //            if (childResult.IsSuccess == true)
        //            {
        //                result.Flags |= TransferResultFlags.Success; // Indicates that at least one underlying persister succeeded

        //                if (childResult.Flags.HasFlag(TransferResultFlags.Found))
        //                {
        //                    result.Value = childResult.Value;
        //                    result.ResolvedViaMount = mount;
        //                    return result;
        //                }
        //            }
        //        }
        //    }
        //    if (!anyMounts) result.Flags |= TransferResultFlags.MountNotAvailable;
        //    return result;
    }

    #endregion

    #region Misc

    private readonly ILogger l;

    #endregion

}
public static class ReflectionX // MOVE
{
    public static Type? UnwrapGeneric(this Type type, Type genericType)
    {
        if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == genericType) return type.GetGenericArguments()[0];
        return null;
    }
    public static Type? IfGenericOrDefault(this Type type, Type genericType)
    {
        if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == genericType) return type;
        return null;
    }
}