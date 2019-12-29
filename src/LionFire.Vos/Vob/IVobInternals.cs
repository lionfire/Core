using System;
using System.Collections.Generic;

namespace LionFire.Vos.Internals
{
    public interface IVobInternals
    {

        //IEnumerable<IVobNode> VobNodes { get; } // TODO - for introspection

        VobNode<T> TryGetNextVobNode<T>(bool skipOwn = false) where T : class;
        VobNode<T> TryGetOwnVobNode<T>() where T : class;

        // REVIEW - do I want both a nodeFactory and a valueFactory?  I think so.
        // TODO: Is this Own or at Root?  Specify via parameter?
        VobNode<TInterface> GetOrAddVobNode<TInterface, TImplementation>(Func<IVobNode, TInterface> factory = null) where TInterface : class;

    }

    public static class IVobInternalsExtensions
    {
        #region Upcast

        #region Values

        public static T GetOrAddOwn<T>(this IVob vob)
            where T : class
        {
            var vobI = vob as IVobInternals;
            var vobNode = vobI.GetOrAddVobNode<T, T>();

            return vobNode.Value;
        }

        #endregion

        #region Nodes

        public static VobNode<T> TryGetNextVobNode<T>(this IVob vob, bool skipOwn = false) where T : class
            => (vob as IVobInternals)?.TryGetNextVobNode<T>(skipOwn);

        public static VobNode<T> TryGetOwnVobNode<T>(this IVob vob) where T : class
            => (vob as IVobInternals)?.TryGetOwnVobNode<T>();

        public static VobNode<TInterface> GetOrAddVobNode<TInterface, TImplementation>(this IVob vob, Func<IVobNode, TInterface> factory = null) where TInterface : class
            => (vob as IVobInternals)?.GetOrAddVobNode<TInterface, TImplementation>(factory);

        public static VobNode<TImplementation> GetOrAddVobNode<TImplementation>(this IVob vob, Func<IVobNode, TImplementation> factory = null) where TImplementation : class
            => (vob as IVobInternals)?.GetOrAddVobNode<TImplementation>(factory);

        #endregion

        #endregion

        #region Implied generic parameters

        public static VobNode<TImplementation> GetOrAddVobNode<TImplementation>(this IVobInternals vobI, Func<IVobNode, TImplementation> factory)
            where TImplementation : class
            => vobI.GetOrAddVobNode<TImplementation, TImplementation>(factory);
        
        #endregion

        //if (vobNodesByType == null) vobNodesByType = new ConcurrentDictionary<Type, IVobNode>();
        //return (VobNode<TImplementation>)vobNodesByType.GetOrAdd(typeof(TImplementation),
        //    t => (IVobNode)Activator.CreateInstance(typeof(VobNode<>).MakeGenericType(t),
        //    this, factory ?? (Func<IVobNode, TImplementation>)(vobNode => (TImplementation)Activator.CreateInstance(typeof(TImplementation), vobNode))));
    }
}
