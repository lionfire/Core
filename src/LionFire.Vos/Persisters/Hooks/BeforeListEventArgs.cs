﻿using LionFire.Referencing;
using LionFire.Vos;

namespace LionFire.Persistence.Persisters.Vos;

public class BeforeReadEventArgs
{
    public VosPersister Persister;
    public IVob Vob;
    public IVob HandlerVob;
    public Type ResultType;
    public IReferenceable<IVobReference> Referenceable;
    public HashSet<string>? Flags;
}

public class BeforeListEventArgs : BeforeReadEventArgs
{
    public Type ListingType;

}
