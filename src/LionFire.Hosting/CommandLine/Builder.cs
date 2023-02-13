#nullable enable
using System;
using System.Collections.Generic;

namespace LionFire.Hosting;

public class Builder<T, TImplementation> : IInitializer<T> where TImplementation : class, T, new()
{
    public T Create(T? builder = default)
    {
        builder ??= new TImplementation();
        if (Initializers != null)
        {
            foreach (var initializer in Initializers)
            {
                initializer(builder);
            }
        }
        return builder;
    }
    public void Add(Action<T> a)
    {
        Initializers ??= new();
        Initializers.Add(a);
    }

    protected List<Action<T>>? Initializers { get; set; }
}
