﻿using LionFire.Referencing;
using LionFire.Vos.Mounts;

namespace LionFire.Persistence.Persisters.Vos;

public class VosPersistenceResult : TransferResult
{
    public IReference ResolvedVia { get; }
    public IMount ResolvedViaMount { get; set; }
}
