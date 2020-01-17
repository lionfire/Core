using System;

namespace LionFire.Vos.Internals
{
    public static class IVobInternalsExtensions
    {
        public static IVobInternals Internals(this IVob vob) => vob as IVobInternals;
        public static IVobInternals Internals(this Vob vob) => vob as IVobInternals;
        public static VobNode<TInterface> AddOwnVobNode<TInterface>(this IVobInternals vobI, Func<IVobNode, TInterface> valueFactory = null)
            where TInterface : class
        {
            var result = vobI.TryAddOwnVobNode(valueFactory);
            if (result == null) throw new AlreadyException();
            return result;
        }
    }
}
