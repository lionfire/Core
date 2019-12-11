using System;

namespace LionFire.Persistence.Handles
{
    public interface INamedHandleProviderConfig
    {
        string Name { get; }

        Type ProviderType { get; }

    }
}
