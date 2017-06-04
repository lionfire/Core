using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.DependencyInjection
{
    /// <summary>
    /// Provides values for properties with the [Provide] attribute
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProvider<T>
    {
        T ProvideForObject(object obj);
    }
}
