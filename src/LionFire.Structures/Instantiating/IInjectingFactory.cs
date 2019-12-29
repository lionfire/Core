using System;

namespace LionFire.Structures
{
    public interface IInjectingFactory<T>
    {
        T Create(IServiceProvider serviceProvider, params object[] parameters);
    }
}