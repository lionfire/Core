#nullable enable
using LionFire.Referencing;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Persisters;

public interface IPersisterEvents
{
    Task OnBeforeRetrieve(IRetrieveContext context);

    event Action<IRetrieveContext> BeforeRetrieve;
}

public class PersisterEvents : IPersisterEvents
{
    public Task OnBeforeRetrieve(IRetrieveContext context)
    {
        throw new NotImplementedException();
    }

    public event Action<IRetrieveContext> BeforeRetrieve;
}

public interface IRetrieveContext
{
    IReference? Reference { get; }
    bool IsListing { get; }
    Type? ListingType { get; }
    HashSet<string> Flags { get; }
    public IPersister Persister { get; }
}

public class RetrieveContext<TReference> : IRetrieveContext
    where TReference : class, IReference
{
    public IPersister<TReference>? Persister { get; set; }
    IPersister IRetrieveContext.Persister => Persister;
    public bool IsListing => ListingType != null;
    public Type? ListingType { get; set; }

    public IReferencable<TReference> Referencable { get; set; }

    public TReference? Reference { get => reference ?? Referencable.Reference; set => reference = value; }
    private TReference? reference;

    IReference? IRetrieveContext.Reference => Reference;
    public HashSet<string> Flags { get; set; }
}
