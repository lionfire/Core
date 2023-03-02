#nullable enable
using System;

namespace LionFire.Hosting
{
    public interface IInitializer<T>
    {
        void Add(Action<T> a);
        T Create(T? builder = default);
    }
}