using System;

namespace LionFire.Structures
{
    public static class LazyExtensions // MOVE
    {
        public static Lazy<T> ToLazy<T>(this Func<T> func) => new Lazy<T>(func);
    }
}
