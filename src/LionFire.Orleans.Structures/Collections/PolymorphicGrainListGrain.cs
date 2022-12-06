using LionFire.Orleans_.Collections.PolymorphicGrainListGrain;
using LionFire.Structures;
using LionFire.Types.Scanning;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Linq;

namespace LionFire.Orleans_.Collections;


public class PolymorphicGrainListGrain<TGrainItem> : ListGrain<PolymorphicGrainListGrainItem<TGrainItem>>
    , IPolymorphicGrainListGrain<TGrainItem>
    //, IAsyncCreating<TValue>
    //ICreatingAsyncDictionary<string, TGrainItem>
    where TGrainItem : IGrain
{
    //public ICreatingAsyncDictionary<TKey, PolymorphicGrainListGrainItem<TGrainItem>> ListGrain => listGrain;
    //ICreatingAsyncDictionary<string, PolymorphicGrainListGrainItem<TGrainItem>> listGrain { get; }
    //public IGrainFactory GrainFactory { get; }

    public TypeScanner TypeScanner { get; }

    #region Lifecycle

    protected PolymorphicGrainListGrain(TypeScanner typeScanner, IPersistentState<List<PolymorphicGrainListGrainItem<TGrainItem>>> items, IPersistentState<SortedDictionary<DateTime, PolymorphicGrainListGrainItem<TGrainItem>>> deletedItemsState = null)
        : base(items, deletedItemsState)
    {
        TypeScanner = typeScanner;
    }

    #endregion

    #region Add / Create / Instantiate

    #region Add

    public Task Add(TGrainItem item)
    {
        throw new NotImplementedException();
    }

    public Task Add(IPolymorphicGrainListGrainItem<TGrainItem> item)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Create

    static IEnumerable<Type>? createableTypes = null;
    public override Task<IEnumerable<Type>> CreateableTypes()
    {
#warning NEXT: why is there no parameter here?

        Func<Type,bool> param = t => t.IsInterface && !t.Name.StartsWith("OrleansCodeGen");

        var result = TypeScanner.GetAllAssignableTo<TGrainItem>("test",param);
        return Task.FromResult(result);
        //return Task.FromResult(createableTypes ??= TypeScanner.GetAllAssignableTo<TGrainItem>(t => t.IsInterface && !t.Name.StartsWith("OrleansCodeGen")));
        //var wrappedType = typeof(IPolymorphicGrainListGrainItem<>).MakeGenericType(type);
    }

    public Task<TGrainItem> Create(Type type, Action<TGrainItem>? init = null)
    {
        throw new NotImplementedException();
    }

    public Task<PolymorphicGrainListGrainItem<TGrainItem>> Create(Type type, Action<PolymorphicGrainListGrainItem<TGrainItem>>? init = null)
    {
        throw new NotImplementedException();
    }

    //public virtual Task<IEnumerable<Type>> CreateableTypes() => Task.FromResult(Enumerable.Empty<Type>());

    #endregion

    protected PolymorphicGrainListGrainItem<TGrainItem> Instantiate(Type type) => new(Guid.NewGuid().ToString(), type);

    #endregion

    #region Remove

    public Task<bool> Remove(TGrainItem item)
    {
        throw new NotImplementedException();
    }
    public Task<bool> Remove(IPolymorphicGrainListGrainItem<TGrainItem> item)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Enumerable

    //public async Task<IEnumerable<TGrainItem>> GrainItems() 
    //    => (await ListGrain.GetEnumerableAsync().ConfigureAwait(false))
    //            .Select(mi => (TGrainItem)GrainFactory.GetGrain(mi.Type, mi.Id))
    //        ;

    public Task<IEnumerable<PolymorphicGrainListGrainItem<TGrainItem>>> Items()
    {
        throw new NotImplementedException();
    }

    public new async Task<IEnumerable<TGrainItem>> GetEnumerableAsync()
    {
        var result = new List<TGrainItem>();
        foreach (var item in await base.GetEnumerableAsync())
        {
            result.Add(item.GetGrain(GrainFactory));
        }
        return result;
    }
    //Task<IEnumerable<IPolymorphicGrainListGrainItem<TGrainItem>>> IEnumerableAsync<IPolymorphicGrainListGrainItem<TGrainItem>>.GetEnumerableAsync()
    //{
    //    throw new NotImplementedException();
    //}

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