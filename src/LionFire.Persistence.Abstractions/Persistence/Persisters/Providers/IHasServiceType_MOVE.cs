#nullable enable

using System;

namespace LionFire.Types;

public interface IHasServiceType
{
    Type? ServiceType { get; }
}
