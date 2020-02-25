#nullable enable
using LionFire.Structures;

namespace LionFire.Persistence
{

    /// <summary>
    /// Wrapper to indicate a type is metadata, and not directly stored by a Persister.  The Persister should persist it via native metadata means, or set aside some privately reserved namespace 
    /// that it can use to store metadata.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Metadata<T> : IWrapper<T>, IMetadata
    {
        public Metadata(T value = default) { Value = value; }
        public T Value { get; set; }

        public static implicit operator Metadata<T>(T value) => new Metadata<T>(value);
    }
}
