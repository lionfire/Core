using DynamicData;
using LionFire.Data.Collections;
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
public class GrainCollectionG<TItemG>
    : DeleteTrackingKeyedCollectionG<string, TItemG>
    where TItemG : IGrainWithStringKey
{
    public TypeScanner TypeScanner { get; } // REFACTOR - Don't have TypeScanner here, but something like ITypeProvider

    #region Lifecycle

    protected GrainCollectionG(ILogger<GrainCollectionG<TItemG>> logger, TypeScanner typeScanner, IPersistentState<Dictionary<string, TItemG>> items, IPersistentState<SortedDictionary<DateTime, string>> deletedItemsState, IServiceProvider serviceProvider)
        : base(serviceProvider, items, logger, deletedItemsState)
    {
        TypeScanner = typeScanner;
    }

    #endregion

    #region Add / Create / Instantiate

    #region Create

    static IEnumerable<Type>? createableTypes = null;
    public override Task<IEnumerable<Type>> SupportedTypes()
    {
        if (createableTypes == null)
        {
            Func<Type, bool> param = t => t.IsInterface && !t.Name.StartsWith("OrleansCodeGen");
#warning NEXT: why is there no parameter here?  parameter gets lost in Orleans RPC somehow?
            createableTypes = TypeScanner.GetAllAssignableTo<TItemG>("test", param);
        }

        return Task.FromResult<IEnumerable<Type>>(createableTypes.ToArray());
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
        ////var id = new string(new GrainType( type, IdSpan.Create());

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

    #endregion

    #region Instantiate

    protected override TItemG Instantiate(Type type) => (TItemG)GrainFactory.GetGrain(type, InstantiateKey());

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