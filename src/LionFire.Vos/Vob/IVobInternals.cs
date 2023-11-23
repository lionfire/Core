using LionFire.MultiTyping;
using System;
using System.Collections.Generic;

namespace LionFire.Vos.Internals
{
    public interface IVobInternals : IMultiTypable
    {
        IEnumerable<KeyValuePair<Type, IVobNode>> VobNodesByType { get; }


        VobNode<T> TryAcquireNextVobNode<T>(int minDepth = 0, int maxDepth = -1) where T : class;

        ContextedVobNode<T> TryGetNextContextedVobNode<T>(int minDepth = 0) where T : class;

        VobNode<T> TryAcquireOwnVobNode<T>() where T : class;

        // REVIEW - do I want both a nodeFactory and a valueFactory?  I think so.
        //VobNode<TInterface> GetOrAddOwnVobNode<TInterface, TImplementation>(Func<IVobNode, TInterface> factory = null)
        //    where TInterface : class
        //    where TImplementation : TInterface;

        VobNode<TInterface> TryAddOwnVobNode<TInterface>(Func<IVobNode, TInterface> valueFactory = null)
            where TInterface : class;

        // RENAME to GetOrAddOwnVobNode, since we're not doing acquisition
        VobNode<TInterface> AcquireOrAddOwnVobNode<TInterface>(Func<IVobNode, TInterface> valueFactory = null)
            where TInterface : class;

        // RENAME to GetOrAddOwnVobNode, since we're not doing acquisition
        VobNode<TInterface> AcquireOrAddOwnVobNode<TInterface, TImplementation>(Func<IVobNode, TInterface> valueFactory = null)
            where TInterface : class
            where TImplementation : TInterface;
        void CleanEmptyChildren(bool recurse = false);
    }
}
