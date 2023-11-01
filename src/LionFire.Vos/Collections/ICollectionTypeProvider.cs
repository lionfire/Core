using System;

namespace LionFire.Vos.Collections;


/// <summary>
/// Resolves the collection type for a particular Vob.
/// </summary>
public interface ICollectionTypeProvider
{
    Type GetCollectionType(IVob vob);
}
