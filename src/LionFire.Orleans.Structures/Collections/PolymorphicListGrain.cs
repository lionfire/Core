
namespace LionFire.Orleans_.Collections;

public abstract class PolymorphicListGrain<TValue, TMetadata> : Grain, IListGrain<TValue, TMetadata>
    where TValue : IGrain
{

    protected IPersistentState<List<PolymorphicListEntry<TMetadata>>> ItemsState { get; }

    public PolymorphicListGrain(/* [PersistentState("Matchmakers", MetaverseStoreNames.Metaverse)] */ IPersistentState<List<PolymorphicListEntry<TMetadata>>> items)
    {
        ItemsState = items;
    }

    public override Task OnActivateAsync()
    {
        ItemsState.State ??= new();
        return base.OnActivateAsync();
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

public abstract class ListGrain<TValue> : Grain, IListGrain<TValue>
    where TValue : IGrain
{

    protected IPersistentState<List<PolymorphicListEntry>> ItemsState { get; }
    protected IPersistentState<List<PolymorphicListEntry>>? DeletedItemsState { get; }

    public ListGrain(/* [PersistentState("Matchmakers", MetaverseStoreNames.Metaverse)] */ IPersistentState<List<PolymorphicListEntry>> items, IPersistentState<List<PolymorphicListEntry>>? deletedItemsState = null)
    {
        ItemsState = items;
        DeletedItemsState = deletedItemsState;
    }

    public override Task OnActivateAsync()
    {
        ItemsState.State ??= new();
        return base.OnActivateAsync();
    }

    public async virtual Task<(string, TValue)> Create(Type type)
    {
        string id;

        do
        {
            id = Guid.NewGuid().ToString();
        } while (ItemsState.State.Where(m => m.Id == id).Any() || (DeletedItemsState != null && DeletedItemsState.State.Where(m => m.Id == id).Any()));

        var item = (TValue)GrainFactory.GetGrain(type, id);

        ItemsState.State.Add(new PolymorphicListEntry(id, type));
        await ItemsState.WriteStateAsync();

        return (id, item);
    }

    public async Task<bool> Remove(string id)
    {
        var existing = ItemsState.State.Where(m => m.Id == id).FirstOrDefault();
        if (existing != null)
        {
            // TOTRANSACTION

            if (DeletedItemsState != null)
            {
                DeletedItemsState.State.Add(existing);
                await DeletedItemsState.WriteStateAsync();
            }

            ItemsState.State.Remove(existing);
            await ItemsState.WriteStateAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public Task<IEnumerable<GrainListItem<TValue>>> Items() => Task.FromResult<IEnumerable<GrainListItem<TValue>>>(ItemsState.State.Select(mi => new GrainListItem<TValue>(mi.Id, mi.Type) { GrainFactory = GrainFactory }).ToList());
    public Task<IEnumerable<GrainListItem<TValue>>> DeletedItems() => Task.FromResult<IEnumerable<GrainListItem<TValue>>>(
        (DeletedItemsState?.State as IEnumerable<PolymorphicListEntry> ?? Enumerable.Empty<PolymorphicListEntry>())
        .Select(mi => new GrainListItem<TValue>(mi.Id, mi.Type) { GrainFactory = GrainFactory })
        .ToList()
        );

    public Task<IEnumerable<PolymorphicListEntry>> GetEnumerable() => Task.FromResult<IEnumerable<PolymorphicListEntry>>(ItemsState.State);

    public virtual Task<IEnumerable<Type>> CreateableTypes() => Task.FromResult(Enumerable.Empty<Type>());

}


//public class MatchmakerListOrig : Grain, IMatchmakerList
//{
//    public IPersistentState<List<MatchmakerInfo>> Matchmakers { get; }

//    public MatchmakerListOrig([PersistentState("Matchmakers", MetaverseStoreNames.Metaverse)] IPersistentState<List<MatchmakerInfo>> matchmakers)
//    {
//        Matchmakers = matchmakers;
//    }

//    public override Task OnActivateAsync()
//    {
//        Matchmakers.State ??= new();
//        return base.OnActivateAsync();
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
