using System;
using LionFire.Vos.Internals;

namespace LionFire.Vos
{
    public static class IVobInternalsVobNodeExtensions
    {

        public static VobNode<TImplementation> GetOrAddVobNode<TImplementation>(this IVobInternals vobI, Func<IVobNode, TImplementation> factory = null)
                where TImplementation : class
            => vobI.GetOrAddOwnVobNode<TImplementation, TImplementation>(factory);


        public static VobNode<TImplementation> GetOrAddVobNode<TImplementation>(this IVobInternals vobI, TImplementation singletonInstance)
            where TImplementation : class
            => vobI.GetOrAddOwnVobNode<TImplementation, TImplementation>(_ => singletonInstance);

        //#region Implied generic parameters

        //public static VobNode<TImplementation> GetOrAddVobNode<TImplementation>(this IVobInternals vobI, Func<IVobNode, TImplementation> factory)
        //    where TImplementation : class
        //    => vobI.GetOrAddOwnVobNode<TImplementation, TImplementation>(factory);

        //#endregion

        #region Upcast

        #region Values

        public static T GetOrAddOwn<T>(this IVob vob, Func<IVobNode, T> valueFactory = null)
            where T : class
            => (vob as IVobInternals).GetOrAddOwnVobNode<T>(valueFactory).Value;

        #endregion

        #region Nodes

        public static VobNode<T> TryGetNextVobNode<T>(this IVob vob, bool skipOwn = false) where T : class
            => (vob as IVobInternals)?.TryGetNextVobNode<T>(skipOwn);

        public static VobNode<T> TryGetOwnVobNode<T>(this IVob vob) where T : class
            => (vob as IVobInternals)?.TryGetOwnVobNode<T>();

        public static VobNode<TInterface> GetOrAddOwnVobNode<TInterface, TImplementation>(this IVob vob, Func<IVobNode, TInterface> factory = null)
            where TInterface : class
            where TImplementation : TInterface
            => (vob as IVobInternals)?.GetOrAddOwnVobNode<TInterface, TImplementation>(factory);

        public static VobNode<TInterface> GetOrAddNextVobNode<TInterface, TImplementation>(this IVob vob, Func<IVobNode, TInterface> factory = null, bool addAtRoot = false)
            where TInterface : class
            where TImplementation : TInterface
        {
            var vobI = vob as IVobInternals;
            var next = vob.TryGetNextVobNode<TInterface>(skipOwn: false);
            if (next != null) return next;

            return (addAtRoot ? vob.Root as IVobInternals : vobI).GetOrAddOwnVobNode<TInterface, TImplementation>(factory);
        }

        public static VobNode<TImplementation> GetOrAddVobNode<TImplementation>(this IVob vob, Func<IVobNode, TImplementation> factory = null) where TImplementation : class
            => (vob as IVobInternals)?.GetOrAddVobNode<TImplementation>(factory);

        #endregion

        #endregion

        

        //if (vobNodesByType == null) vobNodesByType = new ConcurrentDictionary<Type, IVobNode>();
        //return (VobNode<TImplementation>)vobNodesByType.GetOrAdd(typeof(TImplementation),
        //    t => (IVobNode)Activator.CreateInstance(typeof(VobNode<>).MakeGenericType(t),
        //    this, factory ?? (Func<IVobNode, TImplementation>)(vobNode => (TImplementation)Activator.CreateInstance(typeof(TImplementation), vobNode))));
    }

}