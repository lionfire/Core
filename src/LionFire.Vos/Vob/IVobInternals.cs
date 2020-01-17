using LionFire.MultiTyping;
using System;
using System.Collections.Generic;

namespace LionFire.Vos.Internals
{
    public interface IVobInternals : IMultiTypable
    {

        //IEnumerable<IVobNode> VobNodes { get; } // TODO - for introspection

        VobNode<T> TryGetNextVobNode<T>(bool skipOwn = false) where T : class;
        VobNode<T> TryGetOwnVobNode<T>() where T : class;

        // REVIEW - do I want both a nodeFactory and a valueFactory?  I think so.
        //VobNode<TInterface> GetOrAddOwnVobNode<TInterface, TImplementation>(Func<IVobNode, TInterface> factory = null)
        //    where TInterface : class
        //    where TImplementation : TInterface;

        VobNode<TInterface> TryAddOwnVobNode<TInterface>(Func<IVobNode, TInterface> valueFactory = null)
            where TInterface : class;

        VobNode<TInterface> GetOrAddOwnVobNode<TInterface>(Func<IVobNode, TInterface> valueFactory = null)
            where TInterface : class;

        VobNode<TInterface> GetOrAddOwnVobNode<TInterface, TImplementation>(Func<IVobNode, TInterface> valueFactory = null)
            where TInterface : class
            where TImplementation : TInterface;
        
    }
}
