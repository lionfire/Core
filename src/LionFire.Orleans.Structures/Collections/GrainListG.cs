using DynamicData;
using LionFire.Collections.Async;
using LionFire.Ontology;
using LionFire.Structures;
using LionFire.Types.Scanning;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Orleans.Utilities;
using System.Collections;
using System.Linq;
using System.Reactive;

namespace LionFire.Orleans_.Collections;

//[GenerateSerializer]
public class GrainListG<TItemG>
    : KeyedListG<GrainId, TItemG>
    //, IAsyncCreating<TValue>
    //ICreatingAsyncDictionary<string, TItemG>
    where TItemG : IGrainWithStringKey
{
    //public ICreatingAsyncDictionary<TOutputItem, PolymorphicGrainListGrainItem<TItemG>> ListG => listGrain;
    //ICreatingAsyncDictionary<string, PolymorphicGrainListGrainItem<TItemG>> listGrain { get; }
    //public IGrainFactory GrainFactory { get; }

    private readonly ObserverManager<IAsyncObserver<ChangeSet<GrainId>>> collectionNotificationHandlers;

    public TypeScanner TypeScanner { get; }

    #region Lifecycle

    protected GrainListG(ILogger<GrainListG<TItemG>> logger, TypeScanner typeScanner, IPersistentState<List<TItemG>> items, IPersistentState<SortedDictionary<DateTime, TItemG>> deletedItemsState)
        : base(items, deletedItemsState)
    {
        TypeScanner = typeScanner;
        collectionNotificationHandlers = new ObserverManager<IAsyncObserver<ChangeSet<GrainId>>>(TimeSpan.FromMinutes(5), logger);
    }

    #endregion

    #region Add / Create / Instantiate

    #region Add

    public Task Add(TItemG item)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Create

    static IEnumerable<Type>? createableTypes = null;
    public override Task<IEnumerable<Type>> SupportedTypes()
    {
#warning NEXT: why is there no parameter here?

        Func<Type, bool> param = t => t.IsInterface && !t.Name.StartsWith("OrleansCodeGen");

        var result = TypeScanner.GetAllAssignableTo<TItemG>("test", param);
        return Task.FromResult<IEnumerable<Type>>(result.ToArray());
        IEnumerable<Type> list = new List<Type>(result);
        //return Task.FromResult( list);

        //return Task.FromResult(createableTypes ??= TypeScanner.GetAllAssignableTo<TItemG>(t => t.IsInterface && !t.Name.StartsWith("OrleansCodeGen")));
    }

    public async Task<TItemG> Create<TCreateType>(/*, Action<TItemG>? init = null*/)
        where TCreateType : TItemG
    {
        var newGrain = GrainFactory.GetGrain<TCreateType>(Guid.NewGuid().ToString());

        await Add(newGrain).ConfigureAwait(false);

        return newGrain;
        //ItemsState.State.Add(item);
        //await ItemsState.WriteStateAsync();

        //new(Guid.NewGuid().ToString(), type)
        ////var id = new GrainId(new GrainType( type, IdSpan.Create());

        //var item = Instantiate(type);

        //ItemsState.State.Add(item);
        //await ItemsState.WriteStateAsync();
        //return item.GetGrain(GrainFactory);
    }

    //public Task<PolymorphicGrainListGrainItem<TItemG>> Create(Type type/*, Action<PolymorphicGrainListGrainItem<TItemG>>? init = null*/)
    //{
    //    throw new NotImplementedException();
    //}

    //public virtual Task<IEnumerable<Type>> CreateableTypes() => Task.FromResult(Enumerable.Empty<Type>());

    #endregion

    protected override TItemG Instantiate(Type type) => (TItemG)GrainFactory.GetGrain(type, Guid.NewGuid().ToString());

    #endregion

    #region Remove

    //public async Task<bool> Remove(TItemG item) // OLD
    //{
    //    var existing = ItemsState.State.Where(i => i.Id == item.GetGrainId().Key.ToString()).FirstOrDefault();
    //    if (existing == null) return false;

    //    var result = ItemsState.State.Remove(existing);
    //    await ItemsState.WriteStateAsync();
    //    return result;
    //}
    public async Task<bool> Remove(TItemG item)
    {
        var result = ItemsState.State.Remove(item);
        await ItemsState.WriteStateAsync();
        return result;
    }

    #endregion

    #region Enumerable

    //public async Task<IEnumerable<TItemG>> GrainItems() 
    //    => (await ListG.GetEnumerableAsync().ConfigureAwait(false))
    //            .Select(mi => (TItemG)GrainFactory.GetGrain(mi.Type, mi.Id))
    //        ;

    public Task<IEnumerable<TItemG>> Items()
    {
        throw new NotImplementedException();
    }

    public new async Task<IEnumerable<TItemG>> GetEnumerableAsync()
    {
        var result = new List<TItemG>();
        foreach (var item in await base.GetEnumerableAsync())
        {
            result.Add(item);
        }
        return result;
    }

    #endregion



    #region IAsyncObservable

    public ValueTask<IAsyncDisposable> SubscribeAsync(IAsyncObserver<ChangeSet<GrainId>> observer)
    {
        collectionNotificationHandlers.Subscribe((IAddressable)observer, observer);

        return new ValueTask<IAsyncDisposable>(new UnsubscribeOnDispose
        {
            Publisher = this.GetGrainId(),
            Subscriber = (IAddressable)observer,
        });
    }

    public ValueTask UnsubscribeAsync(IAddressable addressable)
    {
        collectionNotificationHandlers.Unsubscribe(addressable);
        return ValueTask.CompletedTask;
    }


    [GenerateSerializer]
    internal class UnsubscribeOnDispose : IAsyncDisposable, IDependsOn<IGrainFactory>
    {
        [Id(0)]
        public GrainId Publisher { get; set; }

        [Id(1)]
        public IAddressable? Subscriber { get; set; }

        public IGrainFactory? GrainFactory { get; set; }
        IGrainFactory IDependsOn<IGrainFactory>.Dependency { set => GrainFactory = value; }

        public ValueTask DisposeAsync()
        {
            ArgumentNullException.ThrowIfNull(GrainFactory, "IDependsOn<IGrainFactory>.Dependency");
            ArgumentNullException.ThrowIfNull(Subscriber);
            var grain = (IChangeSetObservableBaseG)GrainFactory.GetGrain(Publisher);
            grain.UnsubscribeAsync(Subscriber);
            return ValueTask.CompletedTask;
        }
    }

    #endregion

    public Task<bool> Contains(TItemG item)
    {
        throw new NotImplementedException();
    }

    public Task CopyTo(TItemG[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    Task<TItemG> IAsyncListBase<TItemG>.ElementAt(int index)
    {
        throw new NotImplementedException();
    }

    public Task ElementAt(int index, TItemG value)
    {
        throw new NotImplementedException();
    }

    public Task<int> IndexOf(TItemG item)
    {
        throw new NotImplementedException();
    }

    public Task Insert(int index, TItemG item)
    {
        throw new NotImplementedException();
    }

    #region Events

    //public Task Subscribe(ICollectionNotificationObserver observer)
    //{
    //    var obj = GrainFactory.CreateObjectReference<ICollectionNotificationObserver>(observer);

    //    collectionNotificationHandlers.Subscribe(obj, observer);
    //    return Task.CompletedTask;
    //}

    //public Task Unsubscribe(ICollectionNotificationObserver observer)
    //{
    //    collectionNotificationHandlers.Unsubscribe(observer);
    //    return Task.CompletedTask;
    //}

    #endregion

    #region ICreatesG


    public Task<TItemG> Create(Type type, params object[] constructorParameters)
    {
        throw new NotImplementedException();
    }


    #endregion
}

#if OLD

#if UNUSED // with Metadata, probably a bad idea
public abstract class PolymorphicListGrain<TValue, TMetadata> : Grain, IListGrain<TValue, TMetadata>
    where TValue : IGrain
{

    protected IPersistentState<List<PolymorphicListEntry<TMetadata>>> ItemsState { get; }

    public PolymorphicListGrain(/* [PersistentState("Matchmakers", MetaverseStoreNames.Metaverse)] */ IPersistentState<List<PolymorphicListEntry<TMetadata>>> items)
    {
        ItemsState = items;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken = default)
    {
        ItemsState.State ??= new();
        return base.OnActivateAsync(cancellationToken);
    }

    public async virtual Task<(string, TValue)> Create(Type type)
    {
        string id;

        do
        {
            id = Guid.NewGuid().ToString();
        } while (ItemsState.State.Where(m => m.Id == id).Any());

        var item = (TValue)GrainFactory.GetGrain(type, id);

        ItemsState.State.Add(new PolymorphicListEntry<TMetadata>(id, type));
        await ItemsState.WriteStateAsync();

        return (id, item);
    }

    public async Task<bool> Remove(string id)
    {
        var existing = ItemsState.State.Where(m => m.Id == id).FirstOrDefault();
        if (existing != null)
        {
            ItemsState.State.Remove(existing);
            await ItemsState.WriteStateAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public Task<IEnumerable<GrainListItem<TValue, TMetadata>>> Items() => Task.FromResult<IEnumerable<GrainListItem<TValue, TMetadata>>>(ItemsState.State.Select(mi => new GrainListItem<TValue, TMetadata>(mi.Id, mi.Type) { GrainFactory = GrainFactory }).ToList());


    public Task<IEnumerable<PolymorphicListEntry<TMetadata>>> GetEnumerable() => Task.FromResult<IEnumerable<PolymorphicListEntry<TMetadata>>>(ItemsState.State);

    public virtual async Task<IEnumerable<Type>> CreateableTypes()
    {
        var type = await CreateableType();
        return type != null ? (new Type[] { type }) : Enumerable.Empty<Type>();
    }

    public virtual Task<Type?> CreateableType() => Task.FromResult<Type?>(null);

}
#endif


//public class MatchmakerListOrig : Grain, IMatchmakerList
//{
//    public IPersistentState<List<MatchmakerInfo>> Matchmakers { get; }

//    public MatchmakerListOrig([PersistentState("Matchmakers", MetaverseStoreNames.Metaverse)] IPersistentState<List<MatchmakerInfo>> matchmakers)
//    {
//        Matchmakers = matchmakers;
//    }

//    public override Task OnActivateAsync(CancellationToken cancellationToken = default)
//    {
//        Matchmakers.State ??= new();
//        return base.OnActivateAsync(cancellationToken);
//    }

//    public Task<IEnumerable<MatchmakerInfo>> GetAllMatchmakerInfos()
//    {

//        return Task.FromResult((IEnumerable<MatchmakerInfo>)Matchmakers.State);
//    }
//    public Task<IEnumerable<IMatchmaker>> State()
//    {
//        return Task.FromResult<IEnumerable<IMatchmaker>>(Matchmakers.State.Select(mi => (IMatchmaker)GrainFactory.GetGrain(mi.Type, mi.Id)).ToList());
//    }

//    public Task<IEnumerable<string>> GetAllMatchmakerTypes()
//    {
//        var list = new List<string>();
//        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()
//            .Where(a => a.FullName.StartsWith("LionFire") || a.FullName.StartsWith("Valor")))
//        {
//            foreach (var type in assembly.GetTypes()
//                .Where(t =>
//                t.IsInterface
//                && t != typeof(IMatchmaker)
//                && !t.Name.StartsWith("OrleansCodeGen")
//                && t.IsAssignableTo(typeof(IMatchmaker))
//                ))
//            {
//                list.Add(type.AssemblyQualifiedName!);
//            }
//        }
//        return Task.FromResult<IEnumerable<string>>(list);
//    }


//    public async Task<SecuredResponse<IEnumerable<MatchmakerInfo>>> GetMatchmakersForCurrentUser()
//    {
//        if (!this.IsAuthenticated()) return SecuredResponse<IEnumerable<MatchmakerInfo>>.Unauthenticated;
//        if (!(await this.HasPermission(MatchmakerPermissions.ViewAvailableMatchmakers))) return SecuredResponse<IEnumerable<MatchmakerInfo>>.Unauthorized;

//        IEnumerable<MatchmakerInfo> result = Matchmakers.State;

//        if (!(await this.HasPermission(MatchmakerPermissions.ViewHiddenMatchmakers)))
//        {
//            result = result.Where(m => m.Visible);
//        }

//        return (SecuredResponse<IEnumerable<MatchmakerInfo>>)result;
//    }

//    public async Task<(string, IMatchmaker)> Create(Type type)
//    {
//        string id;

//        do
//        {
//            id = Guid.NewGuid().ToString();
//        } while (Matchmakers.State.Where(m => m.Id == id).Any());

//        var matchmaker = (IMatchmaker)GrainFactory.GetGrain(type, id);

//        Matchmakers.State.Add(new MatchmakerInfo
//        {
//            Id = id,
//            Type = type,
//        });
//        await Matchmakers.WriteStateAsync();

//        return (id, matchmaker);
//    }

//    public async Task<bool> Remove(string id)
//    {
//        var existing = Matchmakers.State.Where(m => m.Id == id).FirstOrDefault();
//        if (existing != null)
//        {
//            Matchmakers.State.Remove(existing);
//            await Matchmakers.WriteStateAsync();
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }
//}

#endif