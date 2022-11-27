#nullable enable

using System;

namespace LionFire.Persistence.Persisters;

public interface IMultiTypePersisterProviderOptions
{
    Type? PersisterType { get; set; }
}
