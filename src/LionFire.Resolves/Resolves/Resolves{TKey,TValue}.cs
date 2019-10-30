#if false // UNNEEDED OLD
using LionFire.Structures;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public abstract class ResolvesBase<TKey, TValue> : ResolvesBase<TKey, TValue, TValue>, ILazilyResolves<TValue>
    {
        protected ResolvesBase() { }
        protected ResolvesBase(TKey input) : base(input) { }

        /// <summary>
        /// Do not use this in derived classes that are purely resolve-only and not intended to set an initial value.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="initialValue"></param>
        protected ResolvesBase(TKey input, TValue initialValue) : base(input, initialValue) { }
    }
}
#endif