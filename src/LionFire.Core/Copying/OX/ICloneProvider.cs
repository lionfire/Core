using System;
using System.Collections.Generic;

namespace LionFire
{
    /// <summary>
    /// An interface defining an instance provider, i.e. an object that can create instances of a specific type.
    /// If an instance of a class cannot be deduced automatically by the Copyable framework, and the class
    /// cannot be made a subclass of <see cref="Copyable" />, then creating an instance provider is the preferred
    /// way of making the class copyable.
    /// </summary>
    public interface ICloneProvider
    {
        /// <summary>
        /// The type for which the provider provides instances.
        /// </summary>
        Type Provided { get; }
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="toBeCopied">The object to be copied.</param>
        /// <returns>The created instance.</returns>
        object Clone(object toBeCopied);

        IEnumerable<Type> TypesSupported { get; }
    }

    /// <summary>
    /// The generic version of <see cref="ICloneProvider" />, defining a strongly typed way of providing instances.
    /// </summary>
    /// <typeparam name="T">The type of the instances provided by the implementor.</typeparam>
    public interface ICloneProvider<T> : ICloneProvider
    {
        /// <summary>
        /// Creates a new typed instance.
        /// </summary>
        /// <returns></returns>
        T Clone(T toBeCopied); // TODO: use new operator and use same name
    }
}
