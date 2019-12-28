using System;

namespace LionFire.Vos
{
    public interface IVobInternals
    {

        VobNode<T> TryGetNextVobNode<T>(bool skipOwn = false) where T : class;
        VobNode<T> TryGetOwnVobNode<T>() where T : class;

        VobNode<TInterface> GetOrAddVobNode<TInterface, TImplementation>(Func<IVobNode, TInterface> factory = null) where TInterface : class;

    }

}
