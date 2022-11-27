using LionFire.Referencing;
using LionFire.Vos;

namespace LionFire.Persistence.Persisters.Vos;

public struct BeforeListEventArgs
{
    public VosPersister Persister;
    public IVob Vob;
    public IVob HandlerVob;
    public Type ResultType;
    public Type ListingType;
    public IReferencable<IVobReference> Referencable;
    public HashSet<string>? Flags;
}



//public class BeforeListHandlers
//{
//    public List<Func<IVob, Type, IReferencable<IVobReference>, Task>> Handlers { get; set; }

//    public void AddHandler(Func<IVob, Type, IReferencable<IVobReference>, Task> handler)
//    {
//        Handlers ??= new();
//        Handlers.Add(handler);
//    }

//    public async ValueTask Raise(IVob vob, Type resultType, IReferencable<IVobReference> referencable)
//    {
//        if (Handlers == null) return;
//        foreach (var handler in Handlers)
//        {
//            await handler.Invoke(vob, resultType, referencable).ConfigureAwait(false);
//        }
//    }
//}
//public interface IBeforeList
//{
//    Task OnBeforeList(VosRetrieveContext vosRetrieveContext);
//}

