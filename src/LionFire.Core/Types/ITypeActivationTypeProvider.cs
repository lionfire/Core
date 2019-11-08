using System;

namespace LionFire.Activation
{
    public interface ITypeActivationTypeProvider<T>
    {
        Type Type { get; }
    }
}
